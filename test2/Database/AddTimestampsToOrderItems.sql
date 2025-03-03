-- Thêm cột CreatedAt và UpdatedAt vào bảng OrderItems
ALTER TABLE [dbo].[OrderItems]
ADD 
    [CreatedAt] [datetime] NOT NULL DEFAULT (getdate()),
    [UpdatedAt] [datetime] NOT NULL DEFAULT (getdate())
GO

-- Cập nhật giá trị mặc định cho các bản ghi hiện có
UPDATE [dbo].[OrderItems]
SET 
    [CreatedAt] = getdate(),
    [UpdatedAt] = getdate()
GO 