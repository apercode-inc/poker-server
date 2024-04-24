using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Alter.Column;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Column;
using FluentMigrator.Builders.Create.Index;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.Delete;
using FluentMigrator.Builders.Schema;

namespace server.Code.MorpehFeatures.DataBaseFeature.Utils;

public static class MigrationUtils
	{
		public const string DefaultCollation = "utf8mb4_general_ci";

		public static ICreateTableColumnOptionOrWithColumnSyntax AsProperGuid(
			this ICreateTableColumnAsTypeSyntax expression)
		{
			return expression.AsVarChar(36);
		}

		public static IAlterColumnOptionSyntax AsProperGuid(
			this IAlterColumnAsTypeSyntax expression)
		{
			return expression.AsVarChar(36);
		}

		public static ICreateTableColumnOptionOrWithColumnSyntax ProperForeignKey(
			this ICreateTableColumnOptionOrWithColumnSyntax expression, string table, string column, string name = null)
		{
			return expression.ForeignKey(name, table, column).OnUpdate(Rule.Cascade).OnDelete(Rule.Cascade);
		}
		
		public static ICreateTableColumnOptionOrWithColumnSyntax ProperForeignKey(
			this ICreateTableColumnOptionOrWithColumnSyntax expression, string table, string column, Rule onUpdate, Rule onDelete, string name = null)
		{
			return expression.ForeignKey(name, table, column).OnUpdate(onUpdate).OnDelete(onDelete);
		}

		public static ICreateTableColumnOptionOrWithColumnSyntax AsUInt32(
			this ICreateTableColumnAsTypeSyntax expression)
		{
			return expression.AsCustom("INT UNSIGNED");
		}

		public static ICreateColumnOptionSyntax AsUInt32(this ICreateColumnAsTypeOrInSchemaSyntax expression)
		{
			return expression.AsCustom("INT UNSIGNED");
		}
		
		public static IAlterColumnOptionSyntax AsUInt32(this IAlterColumnAsTypeSyntax expression)
		{
			return expression.AsCustom("INT UNSIGNED");
		}

		public static ICreateTableColumnOptionOrWithColumnSyntax AsUInt16(
			this ICreateTableColumnAsTypeSyntax expression)
		{
			return expression.AsCustom("SMALLINT UNSIGNED");
		}
		
		public static ICreateTableColumnOptionOrWithColumnSyntax AsBinaryArray(
			this ICreateTableColumnAsTypeSyntax expression, int size)
		{
			return expression.AsCustom($"BINARY({size.ToString()})");
		}
		
		public static ICreateColumnOptionSyntax AsUInt16(this ICreateColumnAsTypeOrInSchemaSyntax expression)
		{
			return expression.AsCustom("SMALLINT UNSIGNED");
		}

		public static ICreateTableColumnOptionOrWithColumnSyntax AsText(
			this ICreateTableColumnAsTypeSyntax expression)
		{
			return expression.AsCustom($"TEXT COLLATE {DefaultCollation}");
		}
		
		public static ICreateColumnOptionSyntax AsText(this ICreateColumnAsTypeOrInSchemaSyntax expression)
		{
			return expression.AsCustom($"TEXT COLLATE {DefaultCollation}");
		}

		// AsString with collation overload isn't working. Fix on DB's side?
		public static ICreateTableColumnOptionOrWithColumnSyntax AsVarChar(
			this ICreateTableColumnAsTypeSyntax expression, int size)
		{
			return expression.AsCustom($"VARCHAR({size.ToString()}) COLLATE {DefaultCollation}");
		}
		
		public static ICreateColumnOptionSyntax AsVarChar(
			this ICreateColumnAsTypeOrInSchemaSyntax expression, int size)
		{
			return expression.AsCustom($"VARCHAR({size.ToString()}) COLLATE {DefaultCollation}");
		}

		public static IAlterColumnOptionSyntax AsVarChar(
			this IAlterColumnAsTypeSyntax expression, int size)
		{
			return expression.AsCustom($"VARCHAR({size.ToString()}) COLLATE {DefaultCollation}");
		}

		public static ICreateIndexOnColumnOrInSchemaSyntax OnColumns(
			this ICreateIndexOnColumnOrInSchemaSyntax expression, params string[] columns)
		{
			foreach (var column in columns)
				expression.WithOptions().Clustered().OnColumn(column).Ascending();

			return expression;
		}

		public static ICreateColumnAsTypeOrInSchemaSyntax ColumnIfNotExists(this ICreateExpressionRoot create, ISchemaExpressionRoot schema, string tableName, string columnName)
		{
			return !schema.Table(tableName).Column(columnName).Exists() ? create.Column(columnName).OnTable(tableName) : null;
		}
		
		public static IInSchemaSyntax ColumnsIfExists(this IDeleteExpressionRoot delete, ISchemaExpressionRoot schema, string tableName, params string[] columns)
		{
			IInSchemaSyntax inSchemaSyntax = null;
			
			foreach (var column in columns)
			{
				var schemaSyntax = ColumnIfExists(delete, schema, tableName, column);

				if (schemaSyntax != null)
					inSchemaSyntax = schemaSyntax;
			}

			return inSchemaSyntax;
		}

		public static IInSchemaSyntax ColumnIfExists(this IDeleteExpressionRoot delete, ISchemaExpressionRoot schema, string tableName, string column)
		{
			IInSchemaSyntax inSchemaSyntax = null;
			
			if (schema.Table(tableName).Column(column).Exists())
				inSchemaSyntax = delete.Column(column).FromTable(tableName);

			return inSchemaSyntax;
		}
	}