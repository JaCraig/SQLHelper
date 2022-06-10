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

using Antlr4.Runtime.Misc;
using SQLParser.Parsers.TSql;

namespace SQLHelperDBTests.HelperClasses
{
    /// <summary>
    /// Finds selects within SQL code.
    /// </summary>
    /// <seealso cref="TSqlParserBaseListener"/>
    public class SelectFinder : TSqlParserBaseListener
    {
        /// <summary>
        /// Gets or sets a value indicating whether a select [statement found].
        /// </summary>
        /// <value><c>true</c> if [statement found]; otherwise, <c>false</c>.</value>
        public bool StatementFound { get; set; }

        /// <summary>
        /// Enter a parse tree produced by <see cref="TSqlParser.dml_clause"/>.
        /// <para>The default implementation does nothing.</para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        public override void EnterDml_clause([NotNull] TSqlParser.Dml_clauseContext context)
        {
            var SelectStatement = context?.select_statement_standalone()?.select_statement();
            if (!(SelectStatement is null))
            {
                StatementFound |= !(SelectStatement.query_expression().query_specification() is null);
            }
            base.EnterDml_clause(context);
        }
    }
}