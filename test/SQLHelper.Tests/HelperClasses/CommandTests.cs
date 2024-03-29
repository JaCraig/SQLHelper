﻿using SQLHelperDB.HelperClasses;
using SQLHelperDB.HelperClasses.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Xunit;

namespace SQLHelperDB.Tests.HelperClasses
{
    public class CommandTests
    {
        [Fact]
        public void CanFinalizeAlterTable()
        {
            var Builder = new StringBuilder();
            var TestItem = new Command<int>((__, _, z) => Builder.Append(z), 10, false, "ALTER TABLE [dbo].[SelectOption_] ADD FOREIGN KEY ([User_Creator_ID_]) REFERENCES [dbo].[User_]([ID_]);", CommandType.Text, "@", System.Array.Empty<object>());
            Assert.False(TestItem.Finalizable);
            Assert.True(TestItem.TransactionNeeded);
        }

        [Fact]
        public void CanFinalizeDeclare()
        {
            var Builder = new StringBuilder();
            var TestItem = new Command<int>((__, _, z) => Builder.Append(z), 10, false, "DECLARE @SelectOption_ID_Temp AS BIGINT;", CommandType.Text, "@", System.Array.Empty<object>());
            Assert.False(TestItem.Finalizable);
            Assert.False(TestItem.TransactionNeeded);
            TestItem = new Command<int>((__, _, z) => Builder.Append(z), 10, false, "DECLARE @SelectOption_ID_Temp AS BIGINT;", CommandType.Text, System.Array.Empty<IParameter>());
            Assert.False(TestItem.Finalizable);
            Assert.False(TestItem.TransactionNeeded);
        }

        [Fact]
        public void CanFinalizeDeleteTestNoParameters()
        {
            var Builder = new StringBuilder();
            var TestItem = new Command<int>((__, _, z) => Builder.Append(z), 10, false, "DELETE FROM [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade] WHERE [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_] = @ManyToManyPropertiesWithCascade_ID_ AND NOT (([dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_] = @AllReferencesAndID_ID_0) OR ([dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_] = @AllReferencesAndID_ID_1));", CommandType.Text, "@", System.Array.Empty<object>());
            Assert.False(TestItem.Finalizable);
            Assert.True(TestItem.TransactionNeeded);
        }

        [Fact]
        public void CanFinalizeIfTest()
        {
            var Builder = new StringBuilder();
            var TestItem = new Command<int>((__, _, z) => Builder.Append(z), 10, false, "IF NOT EXISTS (SELECT * FROM [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade] WHERE [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_] = 4 AND [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_] = 4) BEGIN INSERT INTO [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade]([dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_],[dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_]) VALUES (4,4) END;", CommandType.Text, "@", new object[] { 1, 2, 3 });
            Assert.False(TestItem.Finalizable);
            Assert.True(TestItem.TransactionNeeded);
            TestItem = new Command<int>((__, _, z) => Builder.Append(z), 10, false, "IF NOT EXISTS (SELECT * FROM [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade] WHERE [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_] = 4 AND [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_] = 4) BEGIN SELECT * FROM Users END;", CommandType.Text, "@", new object[] { 1, 2, 3 });
            Assert.True(TestItem.Finalizable);
            Assert.False(TestItem.TransactionNeeded);
        }

        [Fact]
        public void CanFinalizeIfTestNoParameters()
        {
            var Builder = new StringBuilder();
            var TestItem = new Command<int>((__, _, z) => Builder.Append(z), 10, false, "IF NOT EXISTS (SELECT * FROM [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade] WHERE [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_] = 4 AND [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_] = 4) BEGIN INSERT INTO [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade]([dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_],[dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_]) VALUES (4,4) END;", CommandType.Text, "@", null);
            Assert.False(TestItem.Finalizable);
            Assert.True(TestItem.TransactionNeeded);
        }

        [Fact]
        public void CanFinalizeMultiLine()
        {
            var Builder = new StringBuilder();
            var TestItem = new Command<int>((__, _, z) => Builder.Append(z), 10, false, @"SELECT
*
FROM Table", CommandType.Text, "@", System.Array.Empty<object>());
            Assert.True(TestItem.Finalizable);
            Assert.False(TestItem.TransactionNeeded);
        }

        [Fact]
        public void CanFinalizeMultipleTest()
        {
            var Builder = new StringBuilder();
            var TestItem = new Command<int>((__, _, z) => Builder.Append(z), 10, false, @"DELETE FROM [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade] WHERE [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_] = 4 AND NOT (([dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_] = 4));
IF NOT EXISTS (SELECT * FROM [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade] WHERE [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_] = 4 AND [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_] = 4) BEGIN INSERT INTO [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade]([dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_],[dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_]) VALUES (4,4) END;
DELETE FROM [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade] WHERE [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_] = 5 AND NOT (([dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_] = 5));
IF NOT EXISTS (SELECT * FROM [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade] WHERE [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_] = 5 AND [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_] = 5) BEGIN INSERT INTO [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade]([dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_],[dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_]) VALUES (5,5) END;
DELETE FROM [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade] WHERE [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_] = 6 AND NOT (([dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_] = 6));
IF NOT EXISTS (SELECT * FROM [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade] WHERE [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_] = 6 AND [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_] = 6) BEGIN INSERT INTO [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade]([dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_],[dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_]) VALUES (6,6) END;", CommandType.Text, "@", new object[] { 1, 2, 3 });
            Assert.False(TestItem.Finalizable);
        }

        //DELETE FROM [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade] WHERE [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_] = @ManyToManyPropertiesWithCascade_ID_ AND NOT (([dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_] = @AllReferencesAndID_ID_0) OR ([dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_] = @AllReferencesAndID_ID_1));
        [Fact]
        public void Creation()
        {
            var TestItem = new Command<int>((___, __, _) => { }, 10, false, "SELECT * FROM TestUsers", CommandType.Text, "@", new object[] { 1, 2, 3 });
            Assert.NotNull(TestItem.CallBack);
            Assert.Equal(CommandType.Text, TestItem.CommandType);
            Assert.True(TestItem.Finalizable);
            Assert.Equal(10, TestItem.CallbackData);
            Assert.Equal(3, TestItem.Parameters.Length);

            for (var x = 0; x < 3; ++x)
            {
                Assert.Equal(DbType.Int32, TestItem.Parameters[x].DatabaseType);
                Assert.Equal(ParameterDirection.Input, TestItem.Parameters[x].Direction);
                Assert.Equal(x.ToString(), TestItem.Parameters[x].ID);
                Assert.Equal(x + 1, TestItem.Parameters[x].InternalValue);
            }
            Assert.Equal("SELECT * FROM TestUsers", TestItem.SQLCommand);
        }

        [Fact]
        public void FinalizeTest()
        {
            var Builder = new StringBuilder();
            var TestItem = new Command<int>((__, _, z) => Builder.Append(z), 10, false, "SELECT * FROM TestUsers", CommandType.Text, "@", new object[] { 1, 2, 3 });
            TestItem.Finalize(new List<dynamic>());
            Assert.Equal("10", Builder.ToString());
        }

        [Fact]
        public void TransactionNeededAlterDatabase()
        {
            var Builder = new StringBuilder();
            var TestItem = new Command<int>((__, _, z) => Builder.Append(z), 10, false, "ALTER DATABASE TestDatabase SET OFFLINE WITH ROLLBACK IMMEDIATE\r\nALTER DATABASE TestDatabase SET ONLINE\r\nDROP DATABASE TestDatabase", CommandType.Text, "@", System.Array.Empty<object>());
            Assert.False(TestItem.TransactionNeeded);
        }

        [Fact]
        public void TransactionNeededCreateDatabase()
        {
            var Builder = new StringBuilder();
            var TestItem = new Command<int>((__, _, z) => Builder.Append(z), 10, false, "Create Database TestDatabase", CommandType.Text, "@", System.Array.Empty<object>());
            Assert.False(TestItem.TransactionNeeded);
        }
    }
}