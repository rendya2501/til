using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqExtension;

/// <summary>
/// Linq拡張
/// </summary>
public static class LinqExtension
{
    /// <summary>
    /// conditionがtrueの時、selectを実行します。
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <param name="condition"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static IEnumerable<TResult> SelectIf<TSource, TResult>(this IEnumerable<TSource> source, bool condition, Func<TSource, TResult> selector)
        => condition ? source.Select(selector) : (IEnumerable<TResult>)source;

    /// <summary>
    /// conditionがtrueの時、whereを実行します。
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="condition"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> WhereIf<TSource>
        (this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
            => condition ? source.Where(predicate) : source;

    /// <summary>
    /// boolによって実行するwhereを切り替えます。
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="condition"></param>
    /// <param name="truePredicate"></param>
    /// <param name="falsePredicate"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> WhereIfElse<TSource>
        (this IEnumerable<TSource> source, bool condition, Func<TSource, bool> truePredicate, Func<TSource, bool> falsePredicate)
            => condition ? source.Where(truePredicate) : source.Where(falsePredicate);
}