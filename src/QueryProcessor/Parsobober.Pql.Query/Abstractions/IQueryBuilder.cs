namespace Parsobober.Pql.Query.Abstractions;

/// <summary>
/// The IQueryBuilder interface defines the methods that a query builder should implement.
/// A query builder is used to construct a PQL query in a step-by-step manner.
/// </summary>
public interface IQueryBuilder
{
    /// <summary>
    /// Builds the PQL query.
    /// </summary>
    /// <returns>The built PQL query.</returns>
    IQueryResult Build();

    /// <summary>
    /// Adds a Select clause to the PQL query.
    /// </summary>
    /// <param name="synonym">The synonym in the Select clause.</param>
    IQueryBuilder AddSelect(string synonym);

    /// <summary>
    /// Set query as a boolean query.
    /// </summary>
    IQueryBuilder SetBoolean();

    IQueryBuilder AddTuple(string tuple);

    /// <summary>
    /// Adds a declaration to the PQL query.
    /// </summary>
    /// <param name="declaration">The declaration to add.</param>
    IQueryBuilder AddDeclaration(string declaration);

    /// <summary>
    /// Adds a With clause to the PQL query.
    /// </summary>
    /// <param name="attribute">The attribute in the With clause.</param>
    /// <param name="reference">The reference in the With clause.</param>
    IQueryBuilder With(string attribute, string reference);

    IQueryBuilder WithCombined(string attribute1, string attribute2);

    #region Relation methods

    /// <summary>
    /// Adds a Follows relationship to the PQL query.
    /// </summary>
    /// <param name="reference1">The first reference in the Follows relationship.</param>
    /// <param name="reference2">The second reference in the Follows relationship.</param>
    IQueryBuilder AddFollows(string reference1, string reference2);

    /// <summary>
    /// Adds a transitive Follows relationship to the PQL query.
    /// </summary>
    /// <param name="reference1">The first reference in the Follows relationship.</param>
    /// <param name="reference2">The second reference in the Follows relationship.</param>
    IQueryBuilder AddFollowsTransitive(string reference1, string reference2);

    /// <summary>
    /// Adds a Parent relationship to the PQL query.
    /// </summary>
    /// <param name="parent">The parent in the Parent relationship.</param>
    /// <param name="child">The child in the Parent relationship.</param>
    IQueryBuilder AddParent(string parent, string child);

    /// <summary>
    /// Adds a transitive Parent relationship to the PQL query.
    /// </summary>
    /// <param name="parent">The parent in the Parent relationship.</param>
    /// <param name="child">The child in the Parent relationship.</param>
    IQueryBuilder AddParentTransitive(string parent, string child);

    /// <summary>
    /// Adds a Modifies relationship to the PQL query.
    /// </summary>
    /// <param name="reference1">The first reference in the Modifies relationship.</param>
    /// <param name="reference2">The second reference in the Modifies relationship.</param>
    IQueryBuilder AddModifies(string reference1, string reference2);

    /// <summary>
    /// Adds a Uses relationship to the PQL query.
    /// </summary>
    /// <param name="reference1">The first reference in the Uses relationship.</param>
    /// <param name="reference2">The second reference in the Uses relationship.</param>
    IQueryBuilder AddUses(string reference1, string reference2);

    IQueryBuilder AddCalls(string reference1, string reference2);

    IQueryBuilder AddCallsTransitive(string reference1, string reference2);

    IQueryBuilder AddNext(string reference1, string reference2);

    IQueryBuilder AddNextTransitive(string reference1, string reference2);

    #endregion
}