using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.Editor;

namespace RequiredProperties
{
    [ExportCodeIssueProvider("RequiredProperties", LanguageNames.CSharp)]
    class CodeIssueProvider : ICodeIssueProvider
    {
        public IEnumerable<CodeIssue> GetIssues(IDocument document, CommonSyntaxNode node, CancellationToken cancellationToken)
        {
            var solution = document.Project.Solution;

            var newExpression = node as ObjectCreationExpressionSyntax;

            var initializer = newExpression.Initializer;

            var types = newExpression.Type;

            var name = types.ToString();

            var symbols = solution.FindSymbols(name).ToList();
                
            yield break;
//            foreach (var token in tokens)
//            {
//                var tokenText = token.ToString();
//
//                if (tokenText.Contains("new"))
//                {
//                    var issueDescription = string.Format("'{0}' contains the letter 'a'", tokenText);
//                    yield return new CodeIssue(CodeIssueKind.Warning, token.Span, issueDescription);
//                }
//            }
        }

        public IEnumerable<Type> SyntaxNodeTypes
        {
            get
            {
                yield return typeof(ObjectCreationExpressionSyntax);
            }
        }

        #region Unimplemented ICodeIssueProvider members

        public IEnumerable<CodeIssue> GetIssues(IDocument document, CommonSyntaxToken token, CancellationToken cancellationToken)
        {
            yield break;
        }

        public IEnumerable<int> SyntaxTokenKinds
        {
            get { yield break; }
        }

        #endregion
    }
}
