using Parsobober.Pql.Query.Abstractions;
using sly.lexer;
using sly.parser.generator;

namespace Parsobober.Pql.Parser;

internal class PqlGrammar(IQueryBuilder queryBuilder)
{
    [Production("select-clause : Declaration* Select[d] Reference condition-clause+")]
    [Production("select-clause : Declaration* Select[d] Boolean condition-clause+")]
    public IQueryBuilder SelectClause(
        List<Token<PqlToken>> declaration, // declarations
        Token<PqlToken> queryType, // reference
        List<IQueryBuilder> _ // condition-clauses
    )
    {
        if (queryType.TokenID == PqlToken.Boolean)
        {
            queryBuilder.SetBoolean();
        }
        else
        {
            queryBuilder.AddSelect(queryType.Value);
        }

        declaration.ForEach(d => queryBuilder.AddDeclaration(d.Value));
        return queryBuilder;
    }

    [Production(
        "select-clause : Declaration* Select[d] LeftAngleBracket[d] tuple-element and-tuple-element* RightAngleBracket[d] condition-clause+")]
    public IQueryBuilder TupleClause(
        List<Token<PqlToken>> declaration, // declarations
        IQueryBuilder element,
        List<IQueryBuilder> _1,
        List<IQueryBuilder> _2 // condition-clauses
    )
    {
        declaration.ForEach(d => queryBuilder.AddDeclaration(d.Value));
        return queryBuilder;
    }

    [Production("and-tuple-element : Coma[d] Reference")]
    public IQueryBuilder TupleElement(Token<PqlToken> reference) => queryBuilder.AddTuple(reference.Value);

    [Production("tuple-element : Reference")]
    public IQueryBuilder TupleReferences(Token<PqlToken> reference) => queryBuilder.AddTuple(reference.Value);

    [Production("condition-clause : such-that-clause")]
    [Production("condition-clause : with-clause")]
    public IQueryBuilder ConditionClause(IQueryBuilder relation) => relation;

    #region With Clause

    [Production("with-clause : With[d] attribute-compare and-attribute-compare*")]
    public IQueryBuilder WithClause(IQueryBuilder compare, List<IQueryBuilder> _) => compare;

    [Production("and-attribute-compare : And[d] attribute-compare")]
    public IQueryBuilder AndAttributeCompare(IQueryBuilder compare) => compare;

    [Production("attribute-compare : Attribute Equal[d] Reference")]
    [Production("attribute-compare : Attribute Equal[d] QuoteReference")]
    public IQueryBuilder AttributeCompare(Token<PqlToken> attribute, Token<PqlToken> reference) =>
        queryBuilder.With(attribute.Value, reference.Value);

    [Production("attribute-compare : Attribute Equal[d] Attribute")]
    public IQueryBuilder AttributeCombine(Token<PqlToken> attribute1, Token<PqlToken> attribute2) =>
        queryBuilder.WithCombined(attribute1.Value, attribute2.Value);

    #endregion


    #region Such That Clause

    [Production("such-that-clause : SuchThat[d] relation and-relation*")]
    public IQueryBuilder SuchThatClause(IQueryBuilder relation, List<IQueryBuilder> _) => relation;

    [Production("and-relation : And[d] relation")]
    public IQueryBuilder AndRelation(IQueryBuilder relation) => relation;

    [Production("relation : Parent[d] LeftParenthesis[d] Reference Coma[d] Reference RightParenthesis[d]")]
    public IQueryBuilder ParentExpression(Token<PqlToken> parent, Token<PqlToken> child) =>
        queryBuilder.AddParent(parent.Value, child.Value);

    [Production("relation : ParentTransitive[d] LeftParenthesis[d] Reference Coma[d] Reference RightParenthesis[d]")]
    public IQueryBuilder ParentTransitiveExpression(Token<PqlToken> parent, Token<PqlToken> child) =>
        queryBuilder.AddParentTransitive(parent.Value, child.Value);

    [Production("relation : Modifies[d] LeftParenthesis[d] Reference Coma[d] Reference RightParenthesis[d]")]
    [Production("relation : Modifies[d] LeftParenthesis[d] QuoteReference Coma[d] Reference RightParenthesis[d]")]
    [Production("relation : Modifies[d] LeftParenthesis[d] Reference Coma[d] QuoteReference RightParenthesis[d]")]
    [Production("relation : Modifies[d] LeftParenthesis[d] QuoteReference Coma[d] QuoteReference RightParenthesis[d]")]
    public IQueryBuilder ModifiesExpression(Token<PqlToken> reference1, Token<PqlToken> reference2) =>
        queryBuilder.AddModifies(reference1.Value, reference2.Value);

    [Production("relation : Follows[d] LeftParenthesis[d] Reference Coma[d] Reference RightParenthesis[d]")]
    public IQueryBuilder FollowsExpression(Token<PqlToken> reference1, Token<PqlToken> reference2) =>
        queryBuilder.AddFollows(reference1.Value, reference2.Value);

    [Production("relation : FollowsTransitive[d] LeftParenthesis[d] Reference Coma[d] Reference RightParenthesis[d]")]
    public IQueryBuilder FollowsTransitiveExpression(Token<PqlToken> reference1, Token<PqlToken> reference2) =>
        queryBuilder.AddFollowsTransitive(reference1.Value, reference2.Value);

    [Production("relation : Uses[d] LeftParenthesis[d] Reference Coma[d] Reference RightParenthesis[d]")]
    [Production("relation : Uses[d] LeftParenthesis[d] QuoteReference Coma[d] Reference RightParenthesis[d]")]
    [Production("relation : Uses[d] LeftParenthesis[d] Reference Coma[d] QuoteReference RightParenthesis[d]")]
    [Production("relation : Uses[d] LeftParenthesis[d] QuoteReference Coma[d] QuoteReference RightParenthesis[d]")]
    public IQueryBuilder UsesExpression(Token<PqlToken> reference1, Token<PqlToken> reference2) =>
        queryBuilder.AddUses(reference1.Value, reference2.Value);

    [Production("relation : Calls[d] LeftParenthesis[d] Reference Coma[d] Reference RightParenthesis[d]")]
    [Production(
        "relation : Calls[d] LeftParenthesis[d] QuoteReference Coma[d] Reference RightParenthesis[d]")]
    [Production(
        "relation : Calls[d] LeftParenthesis[d] Reference Coma[d] QuoteReference RightParenthesis[d]")]
    [Production(
        "relation : Calls[d] LeftParenthesis[d] QuoteReference Coma[d] QuoteReference RightParenthesis[d]")]
    public IQueryBuilder CallsExpression(Token<PqlToken> reference1, Token<PqlToken> reference2) =>
        queryBuilder.AddCalls(reference1.Value, reference2.Value);

    [Production("relation : CallsTransitive[d] LeftParenthesis[d] Reference Coma[d] Reference RightParenthesis[d]")]
    [Production(
        "relation : CallsTransitive[d] LeftParenthesis[d] QuoteReference Coma[d] Reference RightParenthesis[d]")]
    [Production(
        "relation : CallsTransitive[d] LeftParenthesis[d] Reference Coma[d] QuoteReference RightParenthesis[d]")]
    [Production(
        "relation : CallsTransitive[d] LeftParenthesis[d] QuoteReference Coma[d] QuoteReference RightParenthesis[d]")]
    public IQueryBuilder CallsTransitiveExpression(Token<PqlToken> reference1, Token<PqlToken> reference2) =>
        queryBuilder.AddCallsTransitive(reference1.Value, reference2.Value);

    [Production("relation : Next[d] LeftParenthesis[d] Reference Coma[d] Reference RightParenthesis[d]")]
    public IQueryBuilder NextExpression(Token<PqlToken> reference1, Token<PqlToken> reference2) =>
        queryBuilder.AddNext(reference1.Value, reference2.Value);

    [Production("relation : NextTransitive[d] LeftParenthesis[d] Reference Coma[d] Reference RightParenthesis[d]")]
    public IQueryBuilder NextTransitiveExpression(Token<PqlToken> reference1, Token<PqlToken> reference2) =>
        queryBuilder.AddNextTransitive(reference1.Value, reference2.Value);

    #endregion
}