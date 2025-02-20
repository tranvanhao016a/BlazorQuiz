﻿using BlazorQuiz.Api.Data;
using BlazorQuiz.Api.Data.Entities;
using BlazorQuiz.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace BlazorQuiz.Api.Services
{
    public class QuizService
    {
        private readonly QuizContext _context;

        public QuizService(QuizContext context)
        {
            _context = context;
        }

        public async Task<QuizApiResponse> SaveQuizAsync(QuizSaveDto dto)
        {
            var questions = dto.Questions
                   .Select(q => new Question
                   {
                       Id = q.Id,
                       Text = q.Text,
                       Options = q.Options.Select(o => new Option
                       {
                           Id = o.Id,
                           Text = o.Text,
                           IsCorrect = o.IsCorrect
                       }).ToArray()
                   }).ToArray();
            if (dto.Id == Guid.Empty)
            {
                //New Quiz
               
                var quiz = new Quiz
                {
                    CategoryId = dto.CategoryId,
                    IsActive = dto.IsActive,
                    Name = dto.Name,
                    TimeInMinutes = dto.TimeInMinutes,
                    TotalQuestions = dto.TotalQuestions,
                    Questions = questions
                };
                _context.Quizzes.Add(quiz);
            }
            else
            {
                //Update existing Quiz
                var dbQuiz = await _context.Quizzes.FirstOrDefaultAsync(q=> q.Id == dto.Id);
                if(dbQuiz == null)
                {
                    return QuizApiResponse.Fail("Quiz does not exist");
                }
                dbQuiz.CategoryId = dto.CategoryId;
                dbQuiz.IsActive = dto.IsActive;
                dbQuiz.Name = dto.Name;
                dbQuiz.TimeInMinutes = dto.TimeInMinutes;
                dbQuiz.TotalQuestions = dto.TotalQuestions;
                dbQuiz.Questions = questions;

            }
            try
            {
                await _context.SaveChangesAsync();
                return QuizApiResponse.Success;
            }
            catch (Exception ex)
            {
                return QuizApiResponse.Fail(ex.Message);
            }
        }

        public async Task<QuizListDto[]> GetQuizesAsync()
        {
            //Assginment: Implement Paging and Server Side Filter (if required)
           return await _context.Quizzes.Select(q => new QuizListDto
            {
                Id = q.Id,
                Name = q.Name,
                CategoryName = q.Category.Name,
               CategoryId = q.CategoryId,
               TotalQuestions = q.TotalQuestions,
                TimeInMinutes = q.TimeInMinutes,
                IsActive = q.IsActive
            }).ToArrayAsync();
        }

        public async Task<QuestionDto[]> GetQuizQuestions(Guid quizId)
        => await _context.Questions
            .Where(q => q.QuizId == quizId)
            .Select(q => new QuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                //Options = q.Options.Select(o => new OptionDto
                //{
                //    Id = o.Id,
                //    Text = o.Text,
                //    IsCorrect = o.IsCorrect
                //}).ToArray()
            }).ToArrayAsync();

        public async Task<QuizSaveDto> GetQuizToEditAsync(Guid quizId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .Where(q => q.Id == quizId)
                .Select(qz => new QuizSaveDto
                {
                    Id = qz.Id,
                    CategoryId = qz.CategoryId,
                    IsActive = qz.IsActive,
                    Name = qz.Name,
                    TimeInMinutes = qz.TimeInMinutes,
                    TotalQuestions = qz.TotalQuestions,
                    Questions = qz.Questions.Select(q => new QuestionDto
                    {
                        Id = q.Id,
                        Text = q.Text,
                        Options = q.Options.Select(o => new OptionDto
                        {
                            Id = o.Id,
                            Text = o.Text,
                            IsCorrect = o.IsCorrect
                        }).ToList()
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (quiz == null)
            {
                Console.WriteLine("Quiz not found");
            }
            else
            {
                Console.WriteLine($"Quiz found: {quiz.Name}");
            }

            return quiz;
        }


    }
}
