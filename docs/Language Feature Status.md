Language Feature Status
=====

This document reflects the status, and planned work in progress, for the compiler team.  It is a live document
and will be updated as work progresses, features are added / removed, and as work on feature progresses.
This is not an exhaustive list of our features but rather the ones which have active development
efforts behind them.


# C# Next

| Feature                                                                                            | Branch                                                                                  | State                                                        | Developers                                                                   | Reviewer                                                                           | LDM Champ                               |
|----------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------|--------------------------------------------------------------|------------------------------------------------------------------------------|------------------------------------------------------------------------------------|-----------------------------------------|
| [Caller expression attribute](https://github.com/dotnet/csharplang/issues/287)                     | [caller-expression](https://github.com/dotnet/roslyn/tree/features/caller-expression)   | Prototype                                                    | [alrz](https://github.com/alrz)                                              | [jcouv](https://github.com/jcouv)                                                  | [jcouv](https://github.com/jcouv)       |
| [Target-typed new](https://github.com/dotnet/csharplang/issues/100)                                | [target-typed-new](https://github.com/dotnet/roslyn/tree/features/target-typed-new)     | [Prototype](https://github.com/dotnet/roslyn/issues/28489)   | [alrz](https://github.com/alrz)                                              | [jcouv](https://github.com/jcouv)                                                  | [jcouv](https://github.com/jcouv)       |
| [Generic attributes](https://github.com/dotnet/csharplang/issues/124)                              | [generic-attributes](https://github.com/dotnet/roslyn/tree/features/generic-attributes) | [In Progress](https://github.com/dotnet/roslyn/issues/36285) | [AviAvni](https://github.com/AviAvni)                                        | [agocke](https://github.com/agocke)                                                | [mattwar](https://github.com/mattwar)   |
| [Default in deconstruction](https://github.com/dotnet/roslyn/pull/25562)                           | [decon-default](https://github.com/dotnet/roslyn/tree/features/decon-default)           | [Implemented](https://github.com/dotnet/roslyn/issues/25559) | [jcouv](https://github.com/jcouv)                                            | [gafter](https://github.com/gafter)                                                | [jcouv](https://github.com/jcouv)       |
| [Relax ordering of `ref` and `partial` modifiers](https://github.com/dotnet/csharplang/issues/946) | [ref-partial](https://github.com/dotnet/roslyn/tree/features/ref-partial)               | In Progress                                                  | [alrz](https://github.com/alrz)                                              | [gafter](https://github.com/gafter)                                                | [jcouv](https://github.com/jcouv)       |
| [Parameter null-checking](https://github.com/dotnet/csharplang/issues/2145)                        | [param-nullchecking](https://github.com/dotnet/roslyn/tree/features/param-nullchecking) | [In Progress](https://github.com/dotnet/roslyn/issues/36024) | [fayrose](https://github.com/fayrose)                                        | [agocke](https://github.com/agocke)                                                | [jaredpar](https://github.com/jaredpar) |
| [Skip locals init](https://github.com/dotnet/csharplang/issues/1738)                               | [localsinit](https://github.com/dotnet/roslyn/tree/features/localsinit)                 | [In Progress](https://github.com/dotnet/roslyn/issues/25780) | [t-camaia](https://github.com/t-camaia), [agocke](https://github.com/agocke) | [jaredpar](https://github.com/jaredpar)                                            | [agocke](https://github.com/agocke)     |
| [Lambda discard parameters](https://github.com/dotnet/csharplang/issues/111)                       | master                                                                                  | [In Progress](https://github.com/dotnet/roslyn/issues/38820) | [jcouv](https://github.com/jcouv)                                            | [AlekseyTs](https://github.com/AlekseyTs), [chsienki](https://github.com/chsienki) | [jcouv](https://github.com/jcouv)       |
| [Native ints](https://github.com/dotnet/csharplang/issues/435)                                     | TBD                                                                                     | [In Progress](https://github.com/dotnet/roslyn/issues/38821) | [cston](https://github.com/cston)                                            | [333fred](https://github.com/333fred), [gafter](https://github.com/gafter)         | [jaredpar](https://github.com/jaredpar) |
| [Attributes on local functions](https://github.com/dotnet/csharplang/issues/1888)                  | TBD                                                                                     | [In Progress](https://github.com/dotnet/roslyn/issues/38801) | [RikkiGibson](https://github.com/RikkiGibson)                                | [agocke](https://github.com/agocke)                                                | [agocke](https://github.com/agocke)     |
| [Function pointers](https://github.com/dotnet/csharplang/issues/191)                               | [function-pointers](https://github.com/dotnet/roslyn/tree/features/function-pointers)   | [In Progress](https://github.com/dotnet/roslyn/issues/38830) | [333fred](https://github.com/333fred)                                        | [AlekseyTs](https://github.com/AlekseyTs)                                          | [jaredpar](https://github.com/jaredpar) |
| [Pattern matching improvements](https://github.com/dotnet/csharplang/issues/2850)                 | [features/patterns3](https://github.com/dotnet/roslyn/tree/features/patterns3)          | In progress                                                  | [gafter](https://github.com/gafter)                                          | [RikkiGibson](https://github.com/RikkiGibson),[agocke](https://github.com/agocke)  | [gafter](https://github.com/gafter)     |

# C# 8.0

| Feature | Branch | State | Developers | Reviewer | LDM Champ |
| ------- | ------ | ----- | ---------- | -------- | --------- |
| [Default Interface Methods](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-8.0/default-interface-methods.md) | master | [Merged to dev16.1 preview2](https://github.com/dotnet/roslyn/issues/19217) | [AlekseyTs](https://github.com/AlekseyTs) | [gafter](https://github.com/gafter) | [gafter](https://github.com/gafter) |
| [Nullable reference type](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-8.0/nullable-reference-types-specification.md) | master | [Merged to dev16.0 preview1](https://github.com/dotnet/roslyn/issues/22152) | [cston](https://github.com/cston), [jcouv](https://github.com/jcouv) | [AlekseyTs](https://github.com/AlekseyTs), [333fred](https://github.com/333fred) | [mattwar](https://github.com/mattwar) |
| [Recursive patterns](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-8.0/patterns.md) | master | [Merged to dev16.0 preview2](https://github.com/dotnet/roslyn/issues/25935) | [gafter](https://github.com/gafter) | [agocke](https://github.com/agocke), [cston](https://github.com/cston) | [gafter](https://github.com/gafter) |
| [Async streams](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-8.0/async-streams.md) | master | [Merged to dev16.0 preview1](https://github.com/dotnet/roslyn/issues/24037) | [jcouv](https://github.com/jcouv) |  [agocke](https://github.com/agocke) | [stephentoub](https://github.com/stephentoub) |
| [Enhanced using](https://github.com/dotnet/csharplang/issues/1623) | master | [Merged to dev16.0 preview2](https://github.com/dotnet/roslyn/issues/28588) | [chsienki](https://github.com/chsienki) | [agocke](https://github.com/agocke) | [jaredpar](https://github.com/jaredpar) |
| [Ranges](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-8.0/ranges.md) | master | [Merged to dev16.0 preview1](https://github.com/dotnet/roslyn/issues/23205) | [agocke](https://github.com/agocke) | [cston](https://github.com/cston) | [jaredpar](https://github.com/jaredpar) |
| [Null-coalescing Assignment](https://github.com/dotnet/csharplang/issues/34) | master | [Merged to dev16.0 preview1](https://github.com/dotnet/roslyn/issues/29168) | [333fred](https://github.com/333fred) | [cston](https://github.com/cston) | [gafter](https://github.com/gafter)
| [Alternative interpolated verbatim strings](https://github.com/dotnet/csharplang/issues/1630) | master | Merged to dev16.0 preview1 | [jcouv](https://github.com/jcouv) | [cston](https://github.com/cston) | [jcouv](https://github.com/jcouv)
| [stackalloc in nested contexts](https://github.com/dotnet/csharplang/issues/1412) | master | Merged to dev16.2 preview1 | [gafter](https://github.com/gafter) | - | [gafter](https://github.com/gafter)
| [Unmanaged generic structs](https://github.com/dotnet/csharplang/issues/1744) | master | [Merged to dev16.1 preview1](https://github.com/dotnet/roslyn/issues/31374) | [RikkiGibson](https://github.com/RikkiGibson) | - | [jaredpar](https://github.com/jaredpar) |
| [Static local functions](https://github.com/dotnet/csharplang/issues/1565) | master | [Merged in dev16.0 preview2](https://github.com/dotnet/roslyn/issues/32069) | [cston](https://github.com/cston) | [jaredpar](https://github.com/jaredpar) | [jcouv](https://github.com/jcouv)
| [Readonly members](https://github.com/dotnet/csharplang/issues/1710) | master | [Merged to dev16.1 preview2](https://github.com/dotnet/roslyn/issues/32911) | [RikkiGibson](https://github.com/RikkiGibson) | TBD | [jaredpar](https://github.com/jaredpar)

# VB 16.0

| Feature | Branch | State | Developers | Reviewer | LDM Champ |
| ------- | ------ | ----- | ---------- | -------- | --------- |
| [Line continuation comments](https://github.com/dotnet/vblang/issues/65) | [continuation-comments](https://github.com/dotnet/roslyn/tree/features/continuation-comments) | Merged in 16.1 (preview2) | [paul1956](https://github.com/paul1956) | [AlekseyTs](https://github.com/AlekseyTs) | [gafter](https://github.com/gafter) |
| [Relax null-coalescing operator requirements](https://github.com/dotnet/vblang/issues/339) | [null-operator-enhancements](https://github.com/dotnet/roslyn/tree/features/null-operator-enhancements) | Merged in 16.0 | [333fred](https://github.com/333fred) | [cston](https://github.com/cston) | [gafter](https://github.com/gafter) |

# C# 7.3

| Feature | Branch | State | Developers | Reviewer | LDM Champ |
| ------- | ------ | ----- | ---------- | -------- | --------- |
| [blittable](https://github.com/dotnet/csharplang/pull/206) | None | Merged | None | - | [jaredpar](https://github.com/jaredpar) |
| [Support == and != for tuples](https://github.com/dotnet/csharplang/issues/190) | [tuple-equality](https://github.com/dotnet/roslyn/tree/features/tuple-equality) | Merged | [jcouv](https://github.com/jcouv) | [AlekseyTs](https://github.com/AlekseyTs) | [jcouv](https://github.com/jcouv) |
| strongname | master | Merged | [tyoverby](https://github.com/tyoverby) | [agocke](https://github.com/agocke) | [jaredpar](https://github.com/jaredpar) |
| [Attribute on backing field](https://github.com/dotnet/csharplang/issues/42) | features/compiler | Merged | [jcouv](https://github.com/jcouv) | [AlekseyTs](https://github.com/AlekseyTs) | [jcouv](https://github.com/jcouv) |
| Ref Reassignment | [ref-reassignment](https://github.com/dotnet/roslyn/tree/features/ref-reassignment) | Merged | [agocke](https://github.com/agocke) | [vsadov](https://github.com/vsadov) | [gafter](https://github.com/gafter) |
| Constraints | [constraints](https://github.com/dotnet/roslyn/tree/features/constraints) | Merged | [OmarTawfik](https://github.com/OmarTawfik) | [vsadov](https://github.com/vsadov) | [gafter](https://github.com/gafter) |
| [Stackalloc initializers](https://github.com/dotnet/csharplang/issues/1286) | [stackalloc-init](https://github.com/dotnet/roslyn/tree/features/stackalloc-init) | Merged | [alrz](https://github.com/alrz) | [vsadov](https://github.com/vsadov) | [jcouv](https://github.com/jcouv) |
| [Custom fixed](https://github.com/dotnet/csharplang/issues/1314) | [custom-fixed](https://github.com/dotnet/roslyn/tree/features/custom-fixed) | Merged | [vsadov](https://github.com/vsadov) | [jcouv](https://github.com/jcouv) | [jarepdar](https://github.com/jaredpar) |
| [Indexing movable fixed buffers](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-7.3/indexing-movable-fixed-fields.md) | - | Merged | [vsadov](https://github.com/vsadov) | [jcouv](https://github.com/jcouv) | [jarepdar](https://github.com/jaredpar) |
| [Improved overload candidates](https://github.com/dotnet/csharplang/issues/98) | features/compiler | Merged | [gafter](https://github.com/gafter) | [cston](https://github.com/cston) | [mattwar](https://github.com/mattwar) |
| [Expression variables](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-7.3/expression-variables-in-initializers.md) | [features/ExpressionVariables](https://github.com/dotnet/roslyn/tree/features/ExpressionVariables) | Merged | [AlekseyTs](https://github.com/AlekseyTs) | [gafter](https://github.com/gafter) | [AlekseyTs](https://github.com/AlekseyTs) |

# C# 7.2 fixes

| Feature | Branch | State | Developers | Reviewer | LDM Champ |
| ------- | ------ | ----- | ---------- | -------- | --------- |
| [`ref` and `this` ordering in ref extension](https://github.com/dotnet/roslyn/pull/23643) | - | Merged | [alrz](https://github.com/alrz) | - | - |
| [Tiebreaker for by-val and `in` overloads](https://github.com/dotnet/roslyn/pull/23122) | master | Merged | [OmarTawfik](https://github.com/OmarTawfik) | - | - |

# C# 7.2

| Feature | Branch | State | Developers | Reviewer | LDM Champ |
| ------- | ------ | ----- | ---------- | -------- | --------- |
| [ref readonly](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-7.2/readonly-ref.md) | master | Merged | [vsadov](https://github.com/vsadov), [OmarTawfik](https://github.com/OmarTawfik) | [cston](https://github.com/cston),[gafter](https://github.com/gafter) | [jaredpar](https://github.com/jaredpar) |
| [interior pointer/Span<T>/ref struct](https://github.com/dotnet/csharplang/pull/264) | master | Merged | [vsadov](https://github.com/vsadov) | [gafter](https://github.com/gafter), [jaredpar](https://github.com/jaredpar) | [jaredpar](https://github.com/jaredpar) |
| [non-trailing named arguments](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-7.2/non-trailing-named-arguments.md) | master | Merged | [jcouv](https://github.com/jcouv) | [gafter](https://github.com/gafter) | [jcouv](https://github.com/jcouv) |
| [private protected](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-7.2/private-protected.md) | master | Merged | [gafter](https://github.com/gafter) | [jcouv](https://github.com/jcouv) | [gafter](https://github.com/gafter) |
| [conditional ref operator](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-7.2/conditional-ref.md) | master | Merged | [vsadov](https://github.com/vsadov) | [cston](https://github.com/cston) | [jaredpar](https://github.com/jaredpar) |
| [Digit separator after base specifier](https://github.com/dotnet/roslyn/pull/20449) | master | Merged | [alrz](https://github.com/alrz) | - | [gafter](https://github.com/gafter) |

# C# 7.1

| Feature | Branch | State | Developers | Reviewer | LDM Champ |
| ------- | ------ | ----- | ---------- | -------- | --------- |
| [Async Main](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-7.1/async-main.md) | master | Merged | [tyoverby](https://github.com/tyoverby) | [vsadov](https://github.com/vsadov) | [stephentoub](https://github.com/stephentoub) |
| [Default Expressions](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-7.1/target-typed-default.md) | master  | Merged | [jcouv](https://github.com/jcouv) | [cston](https://github.com/cston) | [jcouv](https://github.com/jcouv) |
| [Ref Assemblies](https://github.com/dotnet/roslyn/blob/master/docs/features/refout.md) | master | Merged (IDE and project-system integrations ongoing) | [jcouv](https://github.com/jcouv) | [gafter](https://github.com/gafter) | N/A |
| [Infer tuple names](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-7.1/infer-tuple-names.md) | master | Merged | [jcouv](https://github.com/jcouv) | [gafter](https://github.com/gafter) | [jcouv](https://github.com/jcouv) |
| [Pattern-matching with generics](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-7.1/generics-pattern-match.md) | master  | Merged | [gafter](https://github.com/gafter) | [agocke](https://github.com/agocke) | [gafter](https://github.com/gafter) |

# FAQ

- **Is target version a guarantee?**: No.  It's explicitly not a guarantee.  This is just the planned and ongoing work to the best of our knowledge at this time.
- **Where are these State values defined?**: Take a look at the [Developing a Language Feature](contributing/Developing%20a%20Language%20Feature.md) document.
