﻿using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class QuizQuestion
{
    public int Id { get; set; }

    public string QuestionText { get; set; } = null!;

    public virtual ICollection<QuizAnswer> QuizAnswers { get; set; } = new List<QuizAnswer>();

    public virtual ICollection<UserQuizResponse> UserQuizResponses { get; set; } = new List<UserQuizResponse>();
}
