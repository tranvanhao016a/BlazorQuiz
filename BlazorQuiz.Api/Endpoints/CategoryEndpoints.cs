﻿using BlazorQuiz.Api.Services;
using BlazorQuiz.Shared;
using BlazorQuiz.Shared.DTO;

namespace BlazorQuiz.Api.Endpoints
{
    public static class CategoryEndpoints
    {
        public static IEndpointRouteBuilder MapCatgoryEndpoints(this IEndpointRouteBuilder app)
        {
            var categoryGroup = app.MapGroup("/api/categories")
                .RequireAuthorization();

            categoryGroup.MapGet("", async (CategoryService service) =>
            {
                return await service.GetCategoriesAsync();
            });

            categoryGroup.MapGet("{id}", async (CategoryService service, int id) =>
            {
                return await service.GetCategoryByIdAsync(id);
            });

            categoryGroup.MapPost("", async (CategoryService service, CategoryDto dto) =>
            Results.Ok(await service.CreateCategoryAsync(dto))
            ).RequireAuthorization(p=>p.RequireRole(nameof(UserRole.Admin)));

            return app;
            
        }
    }
}
