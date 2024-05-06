CREATE TABLE [dbo].[Table]
(
	[FirstName] VARCHAR(50) NULL , 
    [LastName] VARCHAR(50) NULL, 
    [Mobile] VARCHAR(50) NOT NULL, 
	[Email] VARBINARY(50) NULL,
	[Catagory] VARBINARY(50) NULL,
    PRIMARY KEY ([Mobile])
)
