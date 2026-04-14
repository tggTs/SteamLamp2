UPDATE [dbo].[Games] 
SET 
    [Description] = ISNULL([Description], 'Описание отсутствует'),
    [ImagePath] = ISNULL([ImagePath], 'Images/default.jpg'),
    [Price] = ISNULL([Price], '0 руб.'),
    [Title] = ISNULL([Title], 'Без названия');
GO