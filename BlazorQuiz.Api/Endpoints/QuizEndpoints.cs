﻿using BlazorQuiz.Api.Data.Entities;
using BlazorQuiz.Api.Services;
using BlazorQuiz.Shared.DTO;

namespace BlazorQuiz.Api.Endpoints
{
    public static class QuizEndpoints
    {
        public static IEndpointRouteBuilder MapQuizEndpoints(this IEndpointRouteBuilder app)
        {
            var quizgroup = app.MapGroup("/api/quizes").RequireAuthorization();

            quizgroup.MapPost("", async ( QuizSaveDto dto, QuizService service) =>
            {
                if(dto.Questions.Count == 0)
                {
                    return Results.BadRequest("Please provide Questions");
                }
                if(dto.Questions.Count != dto.TotalQuestions)
                {
                    return Results.BadRequest("Total Questions count does not match with provided questions");
                }
                return Results.Ok(await service.SaveQuizAsync(dto));
            });

            quizgroup.MapGet("", async (QuizService service) 
                => Results.Ok(await service.GetQuizesAsync()));
            //

            quizgroup.MapGet("{quizId:guid}/questions", async (Guid quizId, QuizService service) =>
                {
                    return Results.Ok(await service.GetQuizQuestions(quizId));
                });

            quizgroup.MapGet("{quizId:guid}", async (Guid quizId, QuizService service) =>
            {
                 return Results.Ok(await service.GetQuizToEditAsync(quizId));
            });

            return app;
        }

    }
}
