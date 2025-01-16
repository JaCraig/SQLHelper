/*
Copyright 2016 James Craig

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using BigBook;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;

namespace SQLHelperDB.HelperClasses
{
    internal class SimpleSelectFinder : SqlCodeObjectVisitor
    {
        public bool StatementFound { get; set; }

        //
        // Summary:
        //     Visit SqlAggregateFunctionCallExpression
        public override void Visit(SqlAggregateFunctionCallExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlAllAnyComparisonBooleanExpression
        public override void Visit(SqlAllAnyComparisonBooleanExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlAllowPageLocksIndexOption
        public override void Visit(SqlAllowPageLocksIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlAllowRowLocksIndexOption
        public override void Visit(SqlAllowRowLocksIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlAssignment
        public override void Visit(SqlAssignment codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlAtTimeZoneExpression
        public override void Visit(SqlAtTimeZoneExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBatch
        public override void Visit(SqlBatch codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBetweenBooleanExpression
        public override void Visit(SqlBetweenBooleanExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBinaryBooleanExpression
        public override void Visit(SqlBinaryBooleanExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBinaryFilterExpression
        public override void Visit(SqlBinaryFilterExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBinaryQueryExpression
        public override void Visit(SqlBinaryQueryExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBinaryScalarExpression
        public override void Visit(SqlBinaryScalarExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBooleanExpression
        public override void Visit(SqlBooleanExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBooleanFilterExpression
        public override void Visit(SqlBooleanFilterExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBuiltinScalarFunctionCallExpression
        public override void Visit(SqlBuiltinScalarFunctionCallExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCastExpression
        public override void Visit(SqlCastExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlChangeTrackingContext
        public override void Visit(SqlChangeTrackingContext codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCheckConstraint
        public override void Visit(SqlCheckConstraint codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlClrAssemblySpecifier
        public override void Visit(SqlClrAssemblySpecifier codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlClrClassSpecifier
        public override void Visit(SqlClrClassSpecifier codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlClrFunctionBodyDefinition
        public override void Visit(SqlClrFunctionBodyDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlClrMethodSpecifier
        public override void Visit(SqlClrMethodSpecifier codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCollateScalarExpression
        public override void Visit(SqlCollateScalarExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCollation
        public override void Visit(SqlCollation codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlColumnAssignment
        public override void Visit(SqlColumnAssignment codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDefaultConstraint
        public override void Visit(SqlDefaultConstraint codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlColumnDefinition
        public override void Visit(SqlColumnDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlColumnIdentity
        public override void Visit(SqlColumnIdentity codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlColumnRefExpression
        public override void Visit(SqlColumnRefExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCommonTableExpression
        public override void Visit(SqlCommonTableExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlComparisonBooleanExpression
        public override void Visit(SqlComparisonBooleanExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCompressionPartitionRange
        public override void Visit(SqlCompressionPartitionRange codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlComputedColumnDefinition
        public override void Visit(SqlComputedColumnDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlConditionClause
        public override void Visit(SqlConditionClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlConstraint
        public override void Visit(SqlConstraint codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlConvertExpression
        public override void Visit(SqlConvertExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateUserOption
        public override void Visit(SqlCreateUserOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCubeGroupByItem
        public override void Visit(SqlCubeGroupByItem codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCursorOption
        public override void Visit(SqlCursorOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCursorVariableAssignment
        public override void Visit(SqlCursorVariableAssignment codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCursorVariableRefExpression
        public override void Visit(SqlCursorVariableRefExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDataCompressionIndexOption
        public override void Visit(SqlDataCompressionIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDataType
        public override void Visit(SqlDataType codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDataTypeSpecification
        public override void Visit(SqlDataTypeSpecification codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDdlTriggerDefinition
        public override void Visit(SqlDdlTriggerDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDefaultValuesInsertMergeActionSource
        public override void Visit(SqlDefaultValuesInsertMergeActionSource codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDefaultValuesInsertSource
        public override void Visit(SqlDefaultValuesInsertSource codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDeleteMergeAction
        public override void Visit(SqlDeleteMergeAction codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDeleteSpecification
        public override void Visit(SqlDeleteSpecification codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDerivedTableExpression
        public override void Visit(SqlDerivedTableExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDistinctPredicateComparisonBooleanExpression
        public override void Visit(SqlDistinctPredicateComparisonBooleanExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDmlSpecificationTableSource
        public override void Visit(SqlDmlSpecificationTableSource codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDmlTriggerDefinition
        public override void Visit(SqlDmlTriggerDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropExistingIndexOption
        public override void Visit(SqlDropExistingIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlExecuteArgument
        public override void Visit(SqlExecuteArgument codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlExecuteAsClause
        public override void Visit(SqlExecuteAsClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlExistsBooleanExpression
        public override void Visit(SqlExistsBooleanExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlFillFactorIndexOption
        public override void Visit(SqlFillFactorIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlFilterClause
        public override void Visit(SqlFilterClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlForBrowseClause
        public override void Visit(SqlForBrowseClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlForeignKeyConstraint
        public override void Visit(SqlForeignKeyConstraint codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlForXmlAutoClause
        public override void Visit(SqlForXmlAutoClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlForXmlClause
        public override void Visit(SqlForXmlClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlForXmlDirective
        public override void Visit(SqlForXmlDirective codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlForXmlExplicitClause
        public override void Visit(SqlForXmlExplicitClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlForXmlPathClause
        public override void Visit(SqlForXmlPathClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlForXmlRawClause
        public override void Visit(SqlForXmlRawClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlFromClause
        public override void Visit(SqlFromClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlFullTextBooleanExpression
        public override void Visit(SqlFullTextBooleanExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlFullTextColumn
        public override void Visit(SqlFullTextColumn codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlFunctionDefinition
        public override void Visit(SqlFunctionDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlGlobalScalarVariableRefExpression
        public override void Visit(SqlGlobalScalarVariableRefExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlGrandTotalGroupByItem
        public override void Visit(SqlGrandTotalGroupByItem codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlGrandTotalGroupingSet
        public override void Visit(SqlGrandTotalGroupingSet codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlGroupByClause
        public override void Visit(SqlGroupByClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlGroupBySets
        public override void Visit(SqlGroupBySets codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlGroupingSetItemsCollection
        public override void Visit(SqlGroupingSetItemsCollection codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlHavingClause
        public override void Visit(SqlHavingClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlIdentifier
        public override void Visit(SqlIdentifier codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlIdentityFunctionCallExpression
        public override void Visit(SqlIdentityFunctionCallExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlIgnoreDupKeyIndexOption
        public override void Visit(SqlIgnoreDupKeyIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlInBooleanExpression
        public override void Visit(SqlInBooleanExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlInBooleanExpressionCollectionValue
        public override void Visit(SqlInBooleanExpressionCollectionValue codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlInBooleanExpressionQueryValue
        public override void Visit(SqlInBooleanExpressionQueryValue codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlIndexedColumn
        public override void Visit(SqlIndexedColumn codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlIndexHint
        public override void Visit(SqlIndexHint codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlIndexOption
        public override void Visit(SqlIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlInlineIndexConstraint
        public override void Visit(SqlInlineIndexConstraint codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlInlineFunctionBodyDefinition
        public override void Visit(SqlInlineFunctionBodyDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlInlineTableRelationalFunctionDefinition
        public override void Visit(SqlInlineTableRelationalFunctionDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlInlineTableVariableDeclaration
        public override void Visit(SqlInlineTableVariableDeclaration codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlInsertMergeAction
        public override void Visit(SqlInsertMergeAction codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlInsertSpecification
        public override void Visit(SqlInsertSpecification codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlIntoClause
        public override void Visit(SqlIntoClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlIsNullBooleanExpression
        public override void Visit(SqlIsNullBooleanExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlJsonObjectArgument
        public override void Visit(SqlJsonObjectArgument codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlLargeDataStorageInformation
        public override void Visit(SqlLargeDataStorageInformation codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlLikeBooleanExpression
        public override void Visit(SqlLikeBooleanExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlLiteralExpression
        public override void Visit(SqlLiteralExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlLoginPassword
        public override void Visit(SqlLoginPassword codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlMaxDegreeOfParallelismIndexOption
        public override void Visit(SqlMaxDegreeOfParallelismIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlMergeActionClause
        public override void Visit(SqlMergeActionClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlMergeSpecification
        public override void Visit(SqlMergeSpecification codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlInsertSource
        public override void Visit(SqlInsertSource codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlModuleCalledOnNullInputOption
        public override void Visit(SqlModuleCalledOnNullInputOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlModuleEncryptionOption
        public override void Visit(SqlModuleEncryptionOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlModuleExecuteAsOption
        public override void Visit(SqlModuleExecuteAsOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlModuleInlineOption
        public override void Visit(SqlModuleInlineOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlModuleNativeCompilationOption
        public override void Visit(SqlModuleNativeCompilationOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlModuleOption
        public override void Visit(SqlModuleOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlModuleRecompileOption
        public override void Visit(SqlModuleRecompileOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlModuleReturnsNullOnNullInputOption
        public override void Visit(SqlModuleReturnsNullOnNullInputOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlModuleSchemaBindingOption
        public override void Visit(SqlModuleSchemaBindingOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlModuleViewMetadataOption
        public override void Visit(SqlModuleViewMetadataOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlMultistatementFunctionBodyDefinition
        public override void Visit(SqlMultistatementFunctionBodyDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlMultistatementTableRelationalFunctionDefinition
        public override void Visit(SqlMultistatementTableRelationalFunctionDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlNotBooleanExpression
        public override void Visit(SqlNotBooleanExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlNullQualifier
        public override void Visit(SqlNullQualifier codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlScalarExpression
        public override void Visit(SqlScalarExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTableExpression
        public override void Visit(SqlTableExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlObjectIdentifier
        public override void Visit(SqlObjectIdentifier codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlObjectReference
        public override void Visit(SqlObjectReference codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlOnlineIndexOption
        public override void Visit(SqlOnlineIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlOptimizeForSequentialKeyIndexOption
        public override void Visit(SqlOptimizeForSequentialKeyIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlResumableIndexOption
        public override void Visit(SqlResumableIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBucketCountIndexOption
        public override void Visit(SqlBucketCountIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCompressionDelayIndexOption
        public override void Visit(SqlCompressionDelayIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlMaxDurationIndexOption
        public override void Visit(SqlMaxDurationIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlOffsetFetchClause
        public override void Visit(SqlOffsetFetchClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlOrderByClause
        public override void Visit(SqlOrderByClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlOrderByItem
        public override void Visit(SqlOrderByItem codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlOutputClause
        public override void Visit(SqlOutputClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlOutputIntoClause
        public override void Visit(SqlOutputIntoClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlPadIndexOption
        public override void Visit(SqlPadIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlParameterDeclaration
        public override void Visit(SqlParameterDeclaration codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlPivotClause
        public override void Visit(SqlPivotClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlPivotTableExpression
        public override void Visit(SqlPivotTableExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlPrimaryKeyConstraint
        public override void Visit(SqlPrimaryKeyConstraint codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlStorageSpecification
        public override void Visit(SqlStorageSpecification codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlProcedureDefinition
        public override void Visit(SqlProcedureDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlQualifiedJoinTableExpression
        public override void Visit(SqlQualifiedJoinTableExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlQueryWithClause
        public override void Visit(SqlQueryWithClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlRollupGroupByItem
        public override void Visit(SqlRollupGroupByItem codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlRowConstructorExpression
        public override void Visit(SqlRowConstructorExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlScalarClrFunctionDefinition
        public override void Visit(SqlScalarClrFunctionDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlScalarFunctionReturnType
        public override void Visit(SqlScalarFunctionReturnType codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlScalarRefExpression
        public override void Visit(SqlScalarRefExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlScalarRelationalFunctionDefinition
        public override void Visit(SqlScalarRelationalFunctionDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlScalarSubQueryExpression
        public override void Visit(SqlScalarSubQueryExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlScalarVariableAssignment
        public override void Visit(SqlScalarVariableAssignment codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlScalarVariableRefExpression
        public override void Visit(SqlScalarVariableRefExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlScript
        public override void Visit(SqlScript codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSearchedCaseExpression
        public override void Visit(SqlSearchedCaseExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSearchedWhenClause
        public override void Visit(SqlSearchedWhenClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSelectIntoClause
        public override void Visit(SqlSelectIntoClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSelectSpecificationInsertSource
        public override void Visit(SqlSelectSpecificationInsertSource codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSelectStarExpression
        public override void Visit(SqlSelectStarExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSelectVariableAssignmentExpression
        public override void Visit(SqlSelectVariableAssignmentExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSetClause
        public override void Visit(SqlSetClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSimpleCaseExpression
        public override void Visit(SqlSimpleCaseExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSimpleGroupByItem
        public override void Visit(SqlSimpleGroupByItem codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSimpleOrderByClause
        public override void Visit(SqlSimpleOrderByClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSimpleOrderByItem
        public override void Visit(SqlSimpleOrderByItem codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSimpleWhenClause
        public override void Visit(SqlSimpleWhenClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSortedDataIndexOption
        public override void Visit(SqlSortedDataIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSortedDataReorgIndexOption
        public override void Visit(SqlSortedDataReorgIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSortInTempDbIndexOption
        public override void Visit(SqlSortInTempDbIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlStatisticsIncrementalIndexOption
        public override void Visit(SqlStatisticsIncrementalIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlStatisticsNoRecomputeIndexOption
        public override void Visit(SqlStatisticsNoRecomputeIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlStatisticsOnlyIndexOption
        public override void Visit(SqlStatisticsOnlyIndexOption codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTableClrFunctionDefinition
        public override void Visit(SqlTableClrFunctionDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTableConstructorExpression
        public override void Visit(SqlTableConstructorExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTableConstructorInsertSource
        public override void Visit(SqlTableConstructorInsertSource codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTableDefinition
        public override void Visit(SqlTableDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTableFunctionReturnType
        public override void Visit(SqlTableFunctionReturnType codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTableHint
        public override void Visit(SqlTableHint codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTableRefExpression
        public override void Visit(SqlTableRefExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTableValuedFunctionRefExpression
        public override void Visit(SqlTableValuedFunctionRefExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTableVariableRefExpression
        public override void Visit(SqlTableVariableRefExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTableUdtInstanceMethodExpression
        public override void Visit(SqlTableUdtInstanceMethodExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTargetTableExpression
        public override void Visit(SqlTargetTableExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTemporalPeriodDefinition
        public override void Visit(SqlTemporalPeriodDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTopSpecification
        public override void Visit(SqlTopSpecification codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTriggerAction
        public override void Visit(SqlTriggerAction codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTriggerDefinition
        public override void Visit(SqlTriggerDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTriggerEvent
        public override void Visit(SqlTriggerEvent codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlUdtInstanceDataMemberExpression
        public override void Visit(SqlUdtInstanceDataMemberExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlUdtInstanceMethodExpression
        public override void Visit(SqlUdtInstanceMethodExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlUdtStaticDataMemberExpression
        public override void Visit(SqlUdtStaticDataMemberExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlUdtStaticMethodExpression
        public override void Visit(SqlUdtStaticMethodExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlUnaryScalarExpression
        public override void Visit(SqlUnaryScalarExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlUniqueConstraint
        public override void Visit(SqlUniqueConstraint codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlUnpivotClause
        public override void Visit(SqlUnpivotClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlUnpivotTableExpression
        public override void Visit(SqlUnpivotTableExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlUnqualifiedJoinTableExpression
        public override void Visit(SqlUnqualifiedJoinTableExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlUpdateBooleanExpression
        public override void Visit(SqlUpdateBooleanExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlUpdateMergeAction
        public override void Visit(SqlUpdateMergeAction codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlUpdateSpecification
        public override void Visit(SqlUpdateSpecification codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlUserDefinedScalarFunctionCallExpression
        public override void Visit(SqlUserDefinedScalarFunctionCallExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlValuesInsertMergeActionSource
        public override void Visit(SqlValuesInsertMergeActionSource codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlVariableColumnAssignment
        public override void Visit(SqlVariableColumnAssignment codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlVariableDeclaration
        public override void Visit(SqlVariableDeclaration codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlViewDefinition
        public override void Visit(SqlViewDefinition codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlWindowClause
        public override void Visit(SqlWindowClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlWindowExpression
        public override void Visit(SqlWindowExpression codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlWindowSpecification
        public override void Visit(SqlWindowSpecification codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlWhereClause
        public override void Visit(SqlWhereClause codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlXmlNamespacesDeclaration
        public override void Visit(SqlXmlNamespacesDeclaration codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlAlterFunctionStatement
        public override void Visit(SqlAlterFunctionStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlAlterLoginStatement
        public override void Visit(SqlAlterLoginStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlAlterProcedureStatement
        public override void Visit(SqlAlterProcedureStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlAlterTriggerStatement
        public override void Visit(SqlAlterTriggerStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlAlterViewStatement
        public override void Visit(SqlAlterViewStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBackupCertificateStatement
        public override void Visit(SqlBackupCertificateStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBackupDatabaseStatement
        public override void Visit(SqlBackupDatabaseStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBackupLogStatement
        public override void Visit(SqlBackupLogStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBackupMasterKeyStatement
        public override void Visit(SqlBackupMasterKeyStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBackupServiceMasterKeyStatement
        public override void Visit(SqlBackupServiceMasterKeyStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBackupTableStatement
        public override void Visit(SqlBackupTableStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlBreakStatement
        public override void Visit(SqlBreakStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCommentStatement
        public override void Visit(SqlCommentStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCompoundStatement
        public override void Visit(SqlCompoundStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlContinueStatement
        public override void Visit(SqlContinueStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateFunctionStatement
        public override void Visit(SqlCreateFunctionStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateIndexStatement
        public override void Visit(SqlCreateIndexStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateLoginFromAsymKeyStatement
        public override void Visit(SqlCreateLoginFromAsymKeyStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateLoginFromCertificateStatement
        public override void Visit(SqlCreateLoginFromCertificateStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateLoginFromWindowsStatement
        public override void Visit(SqlCreateLoginFromWindowsStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateLoginWithPasswordStatement
        public override void Visit(SqlCreateLoginWithPasswordStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateProcedureStatement
        public override void Visit(SqlCreateProcedureStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateRoleStatement
        public override void Visit(SqlCreateRoleStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateSchemaStatement
        public override void Visit(SqlCreateSchemaStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateSynonymStatement
        public override void Visit(SqlCreateSynonymStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateTableStatement
        public override void Visit(SqlCreateTableStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateTriggerStatement
        public override void Visit(SqlCreateTriggerStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateUserDefinedDataTypeStatement
        public override void Visit(SqlCreateUserDefinedDataTypeStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateUserDefinedTableTypeStatement
        public override void Visit(SqlCreateUserDefinedTableTypeStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateUserDefinedTypeStatement
        public override void Visit(SqlCreateUserDefinedTypeStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateUserFromAsymKeyStatement
        public override void Visit(SqlCreateUserFromAsymKeyStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateUserFromCertificateStatement
        public override void Visit(SqlCreateUserFromCertificateStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateUserWithImplicitAuthenticationStatement
        public override void Visit(SqlCreateUserWithImplicitAuthenticationStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateUserFromLoginStatement
        public override void Visit(SqlCreateUserFromLoginStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateUserFromExternalProviderStatement
        public override void Visit(SqlCreateUserFromExternalProviderStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateUserStatement
        public override void Visit(SqlCreateUserStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateUserWithoutLoginStatement
        public override void Visit(SqlCreateUserWithoutLoginStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCreateViewStatement
        public override void Visit(SqlCreateViewStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlCursorDeclareStatement
        public override void Visit(SqlCursorDeclareStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDBCCStatement
        public override void Visit(SqlDBCCStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDeleteStatement
        public override void Visit(SqlDeleteStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDenyStatement
        public override void Visit(SqlDenyStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropAggregateStatement
        public override void Visit(SqlDropAggregateStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropDatabaseStatement
        public override void Visit(SqlDropDatabaseStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropDefaultStatement
        public override void Visit(SqlDropDefaultStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropFunctionStatement
        public override void Visit(SqlDropFunctionStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropLoginStatement
        public override void Visit(SqlDropLoginStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropProcedureStatement
        public override void Visit(SqlDropProcedureStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropRuleStatement
        public override void Visit(SqlDropRuleStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropSchemaStatement
        public override void Visit(SqlDropSchemaStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropSecurityPolicyStatement
        public override void Visit(SqlDropSecurityPolicyStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropSequenceStatement
        public override void Visit(SqlDropSequenceStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropSynonymStatement
        public override void Visit(SqlDropSynonymStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropTableStatement
        public override void Visit(SqlDropTableStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropTriggerStatement
        public override void Visit(SqlDropTriggerStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropTypeStatement
        public override void Visit(SqlDropTypeStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropUserStatement
        public override void Visit(SqlDropUserStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlDropViewStatement
        public override void Visit(SqlDropViewStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlExecuteModuleStatement
        public override void Visit(SqlExecuteModuleStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlExecuteStringStatement
        public override void Visit(SqlExecuteStringStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlGrantStatement
        public override void Visit(SqlGrantStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlIfElseStatement
        public override void Visit(SqlIfElseStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlInlineTableVariableDeclareStatement
        public override void Visit(SqlInlineTableVariableDeclareStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlInsertStatement
        public override void Visit(SqlInsertStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlMergeStatement
        public override void Visit(SqlMergeStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlStatement
        public override void Visit(SqlStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlRestoreDatabaseStatement
        public override void Visit(SqlRestoreDatabaseStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlRestoreInformationStatement
        public override void Visit(SqlRestoreInformationStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlRestoreLogStatement
        public override void Visit(SqlRestoreLogStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlRestoreMasterKeyStatement
        public override void Visit(SqlRestoreMasterKeyStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlRestoreServiceMasterKeyStatement
        public override void Visit(SqlRestoreServiceMasterKeyStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlRestoreTableStatement
        public override void Visit(SqlRestoreTableStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlReturnStatement
        public override void Visit(SqlReturnStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlRevokeStatement
        public override void Visit(SqlRevokeStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSetAssignmentStatement
        public override void Visit(SqlSetAssignmentStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlSetStatement
        public override void Visit(SqlSetStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlTryCatchStatement
        public override void Visit(SqlTryCatchStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlUpdateStatement
        public override void Visit(SqlUpdateStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlUseStatement
        public override void Visit(SqlUseStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlVariableDeclareStatement
        public override void Visit(SqlVariableDeclareStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        //
        // Summary:
        //     Visit SqlWhileStatement
        public override void Visit(SqlWhileStatement codeObject) => codeObject.Children.ForEach(x => x.Accept(this));

        public override void Visit(SqlSelectStatement codeObject)
        {
            StatementFound = true;
            base.Visit(codeObject);
        }
    }
}