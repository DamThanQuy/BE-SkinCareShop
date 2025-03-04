Build started...
Build succeeded.
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Categories] (
    [CategoryID] int NOT NULL IDENTITY,
    [CategoryType] nvarchar(255) NOT NULL,
    [CategoryName] nvarchar(255) NULL,
    CONSTRAINT [PK__Categori__19093A2BFB276FA4] PRIMARY KEY ([CategoryID])
);

CREATE TABLE [ProductCodeCounter] (
    [CategoryType] nvarchar(255) NOT NULL,
    [Counter] int NULL DEFAULT 0,
    CONSTRAINT [PK__ProductC__0BDC1A5976A53B0F] PRIMARY KEY ([CategoryType])
);

CREATE TABLE [QuizQuestions] (
    [Id] int NOT NULL IDENTITY,
    [QuestionText] nvarchar(500) NOT NULL,
    CONSTRAINT [PK__QuizQues__3214EC0722602B9D] PRIMARY KEY ([Id])
);

CREATE TABLE [Users] (
    [UserID] int NOT NULL IDENTITY,
    [Name] nvarchar(255) NOT NULL,
    [FullName] nvarchar(255) NOT NULL,
    [Password] nvarchar(255) NOT NULL,
    [Email] nvarchar(255) NOT NULL DEFAULT N'example@example.com',
    [Role] nvarchar(50) NOT NULL,
    [Phone] nvarchar(50) NOT NULL,
    [Address] nvarchar(255) NULL,
    [RegistrationDate] datetime NOT NULL DEFAULT ((getdate())),
    [SkinType] nvarchar(max) NULL,
    CONSTRAINT [PK__Users__1788CCAC1FD6A3C5] PRIMARY KEY ([UserID])
);

CREATE TABLE [Products] (
    [ProductID] int NOT NULL IDENTITY,
    [ProductCode] nvarchar(50) NULL,
    [CategoryID] int NOT NULL,
    [ProductName] nvarchar(255) NOT NULL,
    [Quantity] int NOT NULL,
    [Capacity] nvarchar(50) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Brand] nvarchar(50) NOT NULL,
    [Origin] nvarchar(50) NOT NULL,
    [Status] nvarchar(50) NOT NULL DEFAULT N'Available',
    [ImgURL] nvarchar(255) NOT NULL,
    [SkinType] nvarchar(255) NULL,
    CONSTRAINT [PK__Products__B40CC6EDE1ED90F2] PRIMARY KEY ([ProductID]),
    CONSTRAINT [FK__Products__Catego__398D8EEE] FOREIGN KEY ([CategoryID]) REFERENCES [Categories] ([CategoryID]) ON DELETE CASCADE
);

CREATE TABLE [QuizAnswers] (
    [AnswerID] int NOT NULL IDENTITY,
    [QuestionID] int NOT NULL,
    [AnswerText] nvarchar(500) NOT NULL,
    [SkinType] nvarchar(50) NOT NULL,
    CONSTRAINT [PK__QuizAnsw__D4825024E077FCC3] PRIMARY KEY ([AnswerID]),
    CONSTRAINT [FK__QuizAnswe__Quest__10566F31] FOREIGN KEY ([QuestionID]) REFERENCES [QuizQuestions] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Carts] (
    [CartID] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [Status] nvarchar(50) NOT NULL,
    CONSTRAINT [PK__Carts__51BCD797E7F3909C] PRIMARY KEY ([CartID]),
    CONSTRAINT [FK__Carts__UserID__75A278F5] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID])
);

CREATE TABLE [Conversations] (
    [ConversationID] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [UpdateAt] datetime NOT NULL DEFAULT ((getdate())),
    [LastMessageID] int NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK__Conversa__C050D8971FED5BCC] PRIMARY KEY ([ConversationID]),
    CONSTRAINT [FK__Conversat__UserI__74AE54BC] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID])
);

CREATE TABLE [Notifications] (
    [NotificationID] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [Title] nvarchar(255) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [Type] nvarchar(50) NOT NULL,
    [IsRead] bit NOT NULL,
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [RelatedID] int NOT NULL,
    [RelatedType] nvarchar(50) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK__Notifica__20CF2E321C04B835] PRIMARY KEY ([NotificationID]),
    CONSTRAINT [FK__Notificat__UserI__73BA3083] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID])
);

CREATE TABLE [Orders] (
    [OrderID] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [OrderDate] datetime NOT NULL DEFAULT ((getdate())),
    [OrderStatus] nvarchar(50) NOT NULL DEFAULT N'Pending',
    [DeliveryStatus] nvarchar(50) NOT NULL DEFAULT N'Not Delivered',
    [DeliveryAddress] nvarchar(255) NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [OrderDelivery] nvarchar(50) NOT NULL,
    [WardCode] nvarchar(50) NOT NULL,
    [WardName] nvarchar(255) NOT NULL,
    [ToDistrictID] int NOT NULL,
    [Note] nvarchar(max) NOT NULL,
    CONSTRAINT [PK__Orders__C3905BAF03C50477] PRIMARY KEY ([OrderID]),
    CONSTRAINT [FK__Orders__UserID__6FE99F9F] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID])
);

CREATE TABLE [UserSkinTypeResults] (
    [ResultID] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [AttemptNumber] int NOT NULL,
    [SkinType] nvarchar(50) NOT NULL,
    [CreatedAt] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__UserSkin__9769022841750638] PRIMARY KEY ([ResultID]),
    CONSTRAINT [FK__UserSkinT__UserI__1AD3FDA4] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID]) ON DELETE CASCADE
);

CREATE TABLE [Vouchers] (
    [VoucherID] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [VoucherName] nvarchar(255) NOT NULL,
    [StartDate] datetime NOT NULL DEFAULT ((getdate())),
    [EndDate] datetime NOT NULL DEFAULT ((getdate())),
    [Description] nvarchar(max) NOT NULL,
    [Status] nvarchar(50) NOT NULL DEFAULT N'Active',
    [Condition] nvarchar(50) NOT NULL,
    [Quantity] int NOT NULL,
    CONSTRAINT [PK__Vouchers__3AEE79C1BECE8D3F] PRIMARY KEY ([VoucherID]),
    CONSTRAINT [FK__Vouchers__UserID__7E37BEF6] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID])
);

CREATE TABLE [Promotions] (
    [PromotionID] int NOT NULL IDENTITY,
    [ProductID] int NOT NULL,
    [EventName] nvarchar(255) NOT NULL,
    [StartDate] datetime NOT NULL,
    [EndDate] datetime NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Status] nvarchar(50) NOT NULL DEFAULT N'Active',
    [Condition] nvarchar(50) NOT NULL,
    CONSTRAINT [PK__Promotio__52C42F2F07A5F6BC] PRIMARY KEY ([PromotionID]),
    CONSTRAINT [FK__Promotion__Produ__778AC167] FOREIGN KEY ([ProductID]) REFERENCES [Products] ([ProductID])
);

CREATE TABLE [Reviews] (
    [ReviewID] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [ProductID] int NOT NULL,
    [Rating] int NOT NULL,
    [ReviewDate] datetime NOT NULL DEFAULT ((getdate())),
    [ReviewComment] nvarchar(max) NOT NULL,
    CONSTRAINT [PK__Reviews__74BC79AE7926511D] PRIMARY KEY ([ReviewID]),
    CONSTRAINT [FK_Reviews_Products] FOREIGN KEY ([ProductID]) REFERENCES [Products] ([ProductID]),
    CONSTRAINT [FK__Reviews__UserID__70DDC3D8] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID])
);

CREATE TABLE [UserQuizResponses] (
    [ResponseID] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [QuestionID] int NOT NULL,
    [SelectedAnswerID] int NOT NULL,
    [AnsweredAt] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__UserQuiz__1AAA640C6AFB8F2A] PRIMARY KEY ([ResponseID]),
    CONSTRAINT [FK__UserQuizR__Quest__151B244E] FOREIGN KEY ([QuestionID]) REFERENCES [QuizQuestions] ([Id]),
    CONSTRAINT [FK__UserQuizR__Selec__160F4887] FOREIGN KEY ([SelectedAnswerID]) REFERENCES [QuizAnswers] ([AnswerID]),
    CONSTRAINT [FK__UserQuizR__UserI__14270015] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID]) ON DELETE CASCADE
);

CREATE TABLE [CartItems] (
    [CartItemID] int NOT NULL IDENTITY,
    [CartID] int NOT NULL,
    [ProductID] int NOT NULL,
    [Quantity] int NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [IsCustomOrder] bit NOT NULL,
    [CreatedAt] datetime NOT NULL,
    [UpdatedAt] datetime NOT NULL,
    [SellerFullName] nvarchar(255) NOT NULL,
    CONSTRAINT [PK__CartItem__488B0B2ABEFD9837] PRIMARY KEY ([CartItemID]),
    CONSTRAINT [FK__CartItems__CartI__7C4F7684] FOREIGN KEY ([CartID]) REFERENCES [Carts] ([CartID]),
    CONSTRAINT [FK__CartItems__Produ__7D439ABD] FOREIGN KEY ([ProductID]) REFERENCES [Products] ([ProductID])
);

CREATE TABLE [Messages] (
    [MessageID] int NOT NULL IDENTITY,
    [ConversationID] int NOT NULL,
    [UserID] int NOT NULL,
    [MessageContent] nvarchar(max) NOT NULL,
    [SendTime] datetime NOT NULL DEFAULT ((getdate())),
    [IsRead] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime NOT NULL,
    [ImageUrl] nvarchar(255) NOT NULL,
    CONSTRAINT [PK__Messages__C87C037C492F516E] PRIMARY KEY ([MessageID]),
    CONSTRAINT [FK_Messages_Conversations] FOREIGN KEY ([ConversationID]) REFERENCES [Conversations] ([ConversationID]),
    CONSTRAINT [FK__Messages__UserID__72C60C4A] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID])
);

CREATE TABLE [CancelRequests] (
    [CancelRequestID] int NOT NULL IDENTITY,
    [OrderID] int NOT NULL,
    [FullName] nvarchar(255) NOT NULL,
    [Phone] nvarchar(50) NOT NULL,
    [Reason] nvarchar(max) NOT NULL,
    [RequestDate] datetime NOT NULL DEFAULT ((getdate())),
    [Status] nvarchar(50) NOT NULL DEFAULT N'Pending',
    CONSTRAINT [PK__CancelRe__150669FEBB62FD0F] PRIMARY KEY ([CancelRequestID]),
    CONSTRAINT [FK__CancelReq__Order__7A672E12] FOREIGN KEY ([OrderID]) REFERENCES [Orders] ([OrderID])
);

CREATE TABLE [OrderItems] (
    [OrderItemID] int NOT NULL IDENTITY,
    [OrderID] int NOT NULL,
    [ProductID] int NOT NULL,
    [Quantity] int NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [ProductName] nvarchar(255) NOT NULL,
    CONSTRAINT [PK__OrderIte__57ED06A1B7319E31] PRIMARY KEY ([OrderItemID]),
    CONSTRAINT [FK__OrderItem__Order__797309D9] FOREIGN KEY ([OrderID]) REFERENCES [Orders] ([OrderID]),
    CONSTRAINT [FK__OrderItem__Produ__787EE5A0] FOREIGN KEY ([ProductID]) REFERENCES [Products] ([ProductID])
);

CREATE TABLE [Payments] (
    [PaymentID] int NOT NULL IDENTITY,
    [OrderID] int NOT NULL,
    [PaymentDate] datetime NOT NULL DEFAULT ((getdate())),
    [Amount] decimal(18,2) NOT NULL,
    [PaymentStatus] nvarchar(50) NOT NULL DEFAULT N'Pending',
    CONSTRAINT [PK__Payments__9B556A585AF3789A] PRIMARY KEY ([PaymentID]),
    CONSTRAINT [FK__Payments__OrderI__71D1E811] FOREIGN KEY ([OrderID]) REFERENCES [Orders] ([OrderID])
);

CREATE INDEX [IX_CancelRequests_OrderID] ON [CancelRequests] ([OrderID]);

CREATE INDEX [IX_CartItems_CartID] ON [CartItems] ([CartID]);

CREATE INDEX [IX_CartItems_ProductID] ON [CartItems] ([ProductID]);

CREATE INDEX [IX_Carts_UserID] ON [Carts] ([UserID]);

CREATE INDEX [IX_Conversations_UserID] ON [Conversations] ([UserID]);

CREATE INDEX [IX_Messages_ConversationID] ON [Messages] ([ConversationID]);

CREATE INDEX [IX_Messages_UserID] ON [Messages] ([UserID]);

CREATE INDEX [IX_Notifications_UserID] ON [Notifications] ([UserID]);

CREATE INDEX [IX_OrderItems_OrderID] ON [OrderItems] ([OrderID]);

CREATE INDEX [IX_OrderItems_ProductID] ON [OrderItems] ([ProductID]);

CREATE INDEX [IX_Orders_UserID] ON [Orders] ([UserID]);

CREATE INDEX [IX_Payments_OrderID] ON [Payments] ([OrderID]);

CREATE INDEX [IX_Products_CategoryID] ON [Products] ([CategoryID]);

CREATE UNIQUE INDEX [UQ_ProductCode] ON [Products] ([ProductCode]) WHERE ([ProductCode] IS NOT NULL);

CREATE INDEX [IX_Promotions_ProductID] ON [Promotions] ([ProductID]);

CREATE INDEX [IX_QuizAnswers_QuestionID] ON [QuizAnswers] ([QuestionID]);

CREATE INDEX [IX_Reviews_ProductID] ON [Reviews] ([ProductID]);

CREATE INDEX [IX_Reviews_UserID] ON [Reviews] ([UserID]);

CREATE INDEX [IX_UserQuizResponses_QuestionID] ON [UserQuizResponses] ([QuestionID]);

CREATE INDEX [IX_UserQuizResponses_SelectedAnswerID] ON [UserQuizResponses] ([SelectedAnswerID]);

CREATE INDEX [IX_UserQuizResponses_UserID] ON [UserQuizResponses] ([UserID]);

CREATE UNIQUE INDEX [UQ__Users__A9D10534B54631B8] ON [Users] ([Email]);

CREATE INDEX [IX_UserSkinTypeResults_UserID] ON [UserSkinTypeResults] ([UserID]);

CREATE INDEX [IX_Vouchers_UserID] ON [Vouchers] ([UserID]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250225195113_UpdateCategories', N'9.0.2');

COMMIT;
GO


