﻿using System.Collections.Immutable;

using Fluxor;

using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.WebApi.Clients.Categories;

namespace SciMaterials.UI.BWASM.States.Categories;

[FeatureState]
public record FilesCategoriesState(ImmutableArray<FileCategory> Categories)
{
    public FilesCategoriesState() : this(ImmutableArray<FileCategory>.Empty) { }

    public DateTime NextServerCall { get; set; }
    public HashSet<TreeFileCategory> Tree { get; init; }
}

public record FileCategory(Guid Id, string Name, string? Description, Guid? ParentId);

public record TreeFileCategory(Guid Id, string Name, string? Description, Guid? ParentId, HashSet<TreeFileCategory> Categories)
{
    public TreeFileCategory(FileCategory source, HashSet<TreeFileCategory> categories)
        : this(source.Id, source.Name, source.Description, source.ParentId, categories)
    {
    }
};

public record struct BuildTree();
public record struct BuildTreeResult(HashSet<TreeFileCategory> Tree);
public record struct LoadCategories();
public record struct LoadCategoriesResult(ImmutableArray<FileCategory> Categories);

public class FilesCategoriesEffects
{
    private readonly IState<FilesCategoriesState> _state;
    private readonly ICategoriesClient _categoriesClient;

    public FilesCategoriesEffects(IState<FilesCategoriesState> state, ICategoriesClient categoriesClient)
    {
        _state = state;
        _categoriesClient = categoriesClient;
    }

    [EffectMethod(typeof(BuildTree))]
    public Task BuildTree(IDispatcher dispatcher)
    {
        if (_state.Value.NextServerCall > DateTime.Now)
        {
            dispatcher.Dispatch(new LoadCategories());
        }

        var categories = _state.Value.Categories;
        if(categories.IsEmpty) return Task.CompletedTask;

        var rootCategories = categories.Where(x => x.ParentId is null).ToList();

        HashSet<TreeFileCategory> tree = new();
        foreach (FileCategory category in rootCategories)
        {
            tree.Add(new(category, BuildBranch(categories, category.Id)));
        }

        dispatcher.Dispatch(new BuildTreeResult(tree));
        return Task.CompletedTask;
    }

    [EffectMethod(typeof(LoadCategories))]
    public async Task LoadCategories(IDispatcher dispatcher)
    {
        var result = await _categoriesClient.GetAllAsync();
        if (!result.Succeeded)
        {
            // TODO: handle failure response
            return;
        }

        var categories = result.Data?.Select(x => new FileCategory(x.Id, x.Name, x.Description, x.ParentId)).ToImmutableArray();
        dispatcher.Dispatch(new LoadCategoriesResult(categories ?? ImmutableArray<FileCategory>.Empty));
    }

    private HashSet<TreeFileCategory> BuildBranch(IReadOnlyCollection<FileCategory> categories, Guid root)
    {
        var inner = categories.Where(x => x.ParentId == root);
        HashSet<TreeFileCategory> branch = new();
        foreach (var category in inner)
        {
            branch.Add(new(category, BuildBranch(categories, category.Id)));
        }

        return branch;
    }
}

public static class FilesCategoriesReducers
{
    [ReducerMethod]
    public static FilesCategoriesState LoadCategories(FilesCategoriesState state, LoadCategoriesResult action)
    {
        return state with
        {
            Categories = action.Categories,
            // TODO: configure server call timeouts
            NextServerCall = DateTime.UtcNow.AddSeconds(40)
        };
    }

    [ReducerMethod]
    public static FilesCategoriesState BuildTree(FilesCategoriesState state, BuildTreeResult action)
    {
        return state with { Tree = action.Tree };
    }
}