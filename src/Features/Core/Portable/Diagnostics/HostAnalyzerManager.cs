﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.Diagnostics.Log;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    /// <summary>
    /// Provides and caches information about diagnostic analyzers such as <see cref="AnalyzerReference"/>, 
    /// <see cref="DiagnosticAnalyzer"/> instance, <see cref="DiagnosticDescriptor"/>s.
    /// Thread-safe.
    /// </summary>
    internal sealed partial class DiagnosticAnalyzerInfoCache
    {
        /// <summary>
        /// This contains vsix info on where <see cref="HostDiagnosticAnalyzerPackage"/> comes from.
        /// </summary>
        private readonly Lazy<ImmutableArray<HostDiagnosticAnalyzerPackage>> _hostDiagnosticAnalyzerPackages;

        /// <summary>
        /// Key is analyzer reference identity <see cref="GetAnalyzerReferenceIdentity(AnalyzerReference)"/>.
        /// 
        /// We use the key to de-duplicate analyzer references if they are referenced from multiple places.
        /// </summary>
        private readonly Lazy<ImmutableDictionary<object, AnalyzerReference>> _hostAnalyzerReferencesMap;

        /// <summary>
        /// Key is the language the <see cref="DiagnosticAnalyzer"/> supports and key for the second map is analyzer reference identity and
        /// <see cref="DiagnosticAnalyzer"/> for that assembly reference.
        /// 
        /// Entry will be lazily filled in.
        /// </summary>
        private readonly ConcurrentDictionary<string, ImmutableDictionary<object, ImmutableArray<DiagnosticAnalyzer>>> _hostDiagnosticAnalyzersPerLanguageMap;

        /// <summary>
        /// Key is analyzer reference identity <see cref="GetAnalyzerReferenceIdentity(AnalyzerReference)"/>.
        /// 
        /// Value is set of <see cref="DiagnosticAnalyzer"/> that belong to the <see cref="AnalyzerReference"/>.
        /// 
        /// We populate it lazily. otherwise, we will bring in all analyzers preemptively
        /// </summary>
        private readonly Lazy<ImmutableDictionary<object, ImmutableArray<DiagnosticAnalyzer>>> _lazyHostDiagnosticAnalyzersPerReferenceMap;

        /// <summary>
        /// Host diagnostic update source for analyzer host specific diagnostics.
        /// </summary>
        private readonly AbstractHostDiagnosticUpdateSource? _hostDiagnosticUpdateSource;

        /// <summary>
        /// map to compiler diagnostic analyzer.
        /// </summary>
        private ImmutableDictionary<string, DiagnosticAnalyzer> _compilerDiagnosticAnalyzerMap;

        /// <summary>
        /// map from host diagnostic analyzer to package name it came from
        /// </summary>
        private ImmutableDictionary<DiagnosticAnalyzer, string> _hostDiagnosticAnalyzerPackageNameMap;

        /// <summary>
        /// map to compiler diagnostic analyzer descriptor.
        /// </summary>
        private ImmutableDictionary<DiagnosticAnalyzer, HashSet<string>> _compilerDiagnosticAnalyzerDescriptorMap;

        /// <summary>
        /// Cache from <see cref="DiagnosticAnalyzer"/> instance to its supported descriptors.
        /// </summary>
        private readonly ConditionalWeakTable<DiagnosticAnalyzer, IReadOnlyCollection<DiagnosticDescriptor>> _descriptorCache;

        public DiagnosticAnalyzerInfoCache(Lazy<ImmutableArray<HostDiagnosticAnalyzerPackage>> hostAnalyzerPackages, IAnalyzerAssemblyLoader? hostAnalyzerAssemblyLoader, AbstractHostDiagnosticUpdateSource? hostDiagnosticUpdateSource, PrimaryWorkspace primaryWorkspace)
            : this(new Lazy<ImmutableArray<AnalyzerReference>>(() => CreateAnalyzerReferencesFromPackages(hostAnalyzerPackages.Value, new HostAnalyzerReferenceDiagnosticReporter(hostDiagnosticUpdateSource, primaryWorkspace), hostAnalyzerAssemblyLoader), isThreadSafe: true),
                   hostAnalyzerPackages, hostDiagnosticUpdateSource)
        {
        }

        private DiagnosticAnalyzerInfoCache(
            Lazy<ImmutableArray<AnalyzerReference>> hostAnalyzerReferences, Lazy<ImmutableArray<HostDiagnosticAnalyzerPackage>> hostAnalyzerPackages, AbstractHostDiagnosticUpdateSource? hostDiagnosticUpdateSource)
        {
            _hostDiagnosticAnalyzerPackages = hostAnalyzerPackages;
            _hostDiagnosticUpdateSource = hostDiagnosticUpdateSource;

            _hostAnalyzerReferencesMap = new Lazy<ImmutableDictionary<object, AnalyzerReference>>(() => hostAnalyzerReferences.Value.IsDefaultOrEmpty ? ImmutableDictionary<object, AnalyzerReference>.Empty : CreateAnalyzerReferencesMap(hostAnalyzerReferences.Value), isThreadSafe: true);
            _hostDiagnosticAnalyzersPerLanguageMap = new ConcurrentDictionary<string, ImmutableDictionary<object, ImmutableArray<DiagnosticAnalyzer>>>(concurrencyLevel: 2, capacity: 2);
            _lazyHostDiagnosticAnalyzersPerReferenceMap = new Lazy<ImmutableDictionary<object, ImmutableArray<DiagnosticAnalyzer>>>(() => CreateDiagnosticAnalyzersPerReferenceMap(_hostAnalyzerReferencesMap.Value), isThreadSafe: true);

            _compilerDiagnosticAnalyzerMap = ImmutableDictionary<string, DiagnosticAnalyzer>.Empty;
            _compilerDiagnosticAnalyzerDescriptorMap = ImmutableDictionary<DiagnosticAnalyzer, HashSet<string>>.Empty;
            _hostDiagnosticAnalyzerPackageNameMap = ImmutableDictionary<DiagnosticAnalyzer, string>.Empty;
            _descriptorCache = new ConditionalWeakTable<DiagnosticAnalyzer, IReadOnlyCollection<DiagnosticDescriptor>>();
        }

        // this is for testing
        internal DiagnosticAnalyzerInfoCache(ImmutableArray<AnalyzerReference> hostAnalyzerReferences, AbstractHostDiagnosticUpdateSource hostDiagnosticUpdateSource)
            : this(new Lazy<ImmutableArray<AnalyzerReference>>(() => hostAnalyzerReferences), new Lazy<ImmutableArray<HostDiagnosticAnalyzerPackage>>(() => ImmutableArray<HostDiagnosticAnalyzerPackage>.Empty), hostDiagnosticUpdateSource)
        {
        }

        /// <summary>
        /// It returns a string that can be used as a way to de-duplicate <see cref="AnalyzerReference"/>s.
        /// </summary>
        public object GetAnalyzerReferenceIdentity(AnalyzerReference reference)
        {
            return reference.Id;
        }

        /// <summary>
        /// It returns a map with <see cref="AnalyzerReference.Id"/> as key and <see cref="AnalyzerReference"/> as value
        /// </summary>
        public ImmutableDictionary<object, AnalyzerReference> CreateAnalyzerReferencesMap(Project? project = null)
        {
            if (project == null)
            {
                return _hostAnalyzerReferencesMap.Value;
            }

            return _hostAnalyzerReferencesMap.Value.AddRange(CreateProjectAnalyzerReferencesMap(project));
        }

        /// <summary>
        /// Return <see cref="DiagnosticAnalyzer.SupportedDiagnostics"/> of given <paramref name="analyzer"/>.
        /// </summary>
        public ImmutableArray<DiagnosticDescriptor> GetDiagnosticDescriptors(DiagnosticAnalyzer analyzer)
        {
            return (ImmutableArray<DiagnosticDescriptor>)_descriptorCache.GetValue(analyzer, a => GetDiagnosticDescriptorsCore(a));
        }

        private ImmutableArray<DiagnosticDescriptor> GetDiagnosticDescriptorsCore(DiagnosticAnalyzer analyzer)
        {
            ImmutableArray<DiagnosticDescriptor> descriptors;
            try
            {
                // SupportedDiagnostics is user code and can throw an exception.
                descriptors = analyzer.SupportedDiagnostics;
                if (descriptors.IsDefault)
                {
                    descriptors = ImmutableArray<DiagnosticDescriptor>.Empty;
                }
            }
            catch (Exception ex)
            {
                AnalyzerHelper.OnAnalyzerExceptionForSupportedDiagnostics(analyzer, ex, _hostDiagnosticUpdateSource);
                descriptors = ImmutableArray<DiagnosticDescriptor>.Empty;
            }

            return descriptors;
        }

        /// <summary>
        /// Return true if the given <paramref name="analyzer"/> is suppressed for the given project.
        /// </summary>
        public bool IsAnalyzerSuppressed(DiagnosticAnalyzer analyzer, Project project)
        {
            var options = project.CompilationOptions;
            if (options == null || IsCompilerDiagnosticAnalyzer(project.Language, analyzer))
            {
                return false;
            }

            // If user has disabled analyzer execution for this project, we only want to execute required analyzers
            // that report diagnostics with category "Compiler".
            if (!project.State.RunAnalyzers &&
                GetDiagnosticDescriptors(analyzer).All(d => d.Category != DiagnosticCategory.Compiler))
            {
                return true;
            }

            // don't capture project
            var projectId = project.Id;

            // Skip telemetry logging for supported diagnostics, as that can cause an infinite loop.
            void onAnalyzerException(Exception ex, DiagnosticAnalyzer a, Diagnostic diagnostic) =>
                    AnalyzerHelper.OnAnalyzerException_NoTelemetryLogging(a, diagnostic, _hostDiagnosticUpdateSource, projectId);

            return CompilationWithAnalyzers.IsDiagnosticAnalyzerSuppressed(analyzer, options, onAnalyzerException);
        }

        /// <summary>
        /// Get <see cref="AnalyzerReference"/> identity and <see cref="DiagnosticAnalyzer"/>s map for given <paramref name="language"/>
        /// </summary> 
        public ImmutableDictionary<object, ImmutableArray<DiagnosticAnalyzer>> GetOrCreateHostDiagnosticAnalyzersPerReference(string language)
        {
            return _hostDiagnosticAnalyzersPerLanguageMap.GetOrAdd(language, CreateHostDiagnosticAnalyzersAndBuildMap);
        }

        /// <summary>
        /// Create <see cref="AnalyzerReference"/> identity and <see cref="DiagnosticDescriptor"/>s map
        /// </summary>
        public ImmutableDictionary<object, ImmutableArray<DiagnosticDescriptor>> GetHostDiagnosticDescriptorsPerReference()
        {
            return CreateDiagnosticDescriptorsPerReference(_lazyHostDiagnosticAnalyzersPerReferenceMap.Value);
        }

        /// <summary>
        /// Create <see cref="AnalyzerReference"/> identity and <see cref="DiagnosticDescriptor"/>s map for given <paramref name="project"/>
        /// </summary>
        public ImmutableDictionary<object, ImmutableArray<DiagnosticDescriptor>> CreateDiagnosticDescriptorsPerReference(Project project)
        {
            return CreateDiagnosticDescriptorsPerReference(CreateDiagnosticAnalyzersPerReference(project));
        }

        /// <summary>
        /// Create <see cref="AnalyzerReference"/> identity and <see cref="DiagnosticAnalyzer"/>s map for given <paramref name="project"/> that
        /// includes both host and project analyzers
        /// </summary>
        public ImmutableDictionary<object, ImmutableArray<DiagnosticAnalyzer>> CreateDiagnosticAnalyzersPerReference(Project project)
        {
            var hostAnalyzerReferences = GetOrCreateHostDiagnosticAnalyzersPerReference(project.Language);
            var projectAnalyzerReferences = CreateProjectDiagnosticAnalyzersPerReference(project);

            return MergeDiagnosticAnalyzerMap(hostAnalyzerReferences, projectAnalyzerReferences);
        }

        /// <summary>
        /// Create <see cref="AnalyzerReference"/> identity and <see cref="DiagnosticAnalyzer"/>s map for given <paramref name="project"/> that
        /// has only project analyzers
        /// </summary>
        public ImmutableDictionary<object, ImmutableArray<DiagnosticAnalyzer>> CreateProjectDiagnosticAnalyzersPerReference(Project project)
        {
            return CreateDiagnosticAnalyzersPerReferenceMap(CreateProjectAnalyzerReferencesMap(project), project.Language);
        }

        /// <summary>
        /// Check whether given <see cref="DiagnosticData"/> belong to compiler diagnostic analyzer
        /// </summary>
        public bool IsCompilerDiagnostic(string language, DiagnosticData diagnostic)
        {
            _ = GetOrCreateHostDiagnosticAnalyzersPerReference(language);
            if (_compilerDiagnosticAnalyzerMap.TryGetValue(language, out var compilerAnalyzer) &&
                _compilerDiagnosticAnalyzerDescriptorMap.TryGetValue(compilerAnalyzer, out var idMap) &&
                idMap.Contains(diagnostic.Id))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Return compiler <see cref="DiagnosticAnalyzer"/> for the given language.
        /// </summary>
        public DiagnosticAnalyzer? GetCompilerDiagnosticAnalyzer(string language)
        {
            _ = GetOrCreateHostDiagnosticAnalyzersPerReference(language);
            if (_compilerDiagnosticAnalyzerMap.TryGetValue(language, out var compilerAnalyzer))
            {
                return compilerAnalyzer;
            }

            return null;
        }

        /// <summary>
        /// Check whether given <see cref="DiagnosticAnalyzer"/> is compiler analyzer for the language or not.
        /// </summary>
        public bool IsCompilerDiagnosticAnalyzer(string language, DiagnosticAnalyzer analyzer)
        {
            _ = GetOrCreateHostDiagnosticAnalyzersPerReference(language);
            return _compilerDiagnosticAnalyzerMap.TryGetValue(language, out var compilerAnalyzer) && compilerAnalyzer == analyzer;
        }

        /// <summary>
        /// Get Name of Package (vsix) which Host <see cref="DiagnosticAnalyzer"/> is from.
        /// </summary>
        public string? GetDiagnosticAnalyzerPackageName(string language, DiagnosticAnalyzer analyzer)
        {
            _ = GetOrCreateHostDiagnosticAnalyzersPerReference(language);
            if (_hostDiagnosticAnalyzerPackageNameMap.TryGetValue(analyzer, out var name))
            {
                return name;
            }

            return null;
        }

        private ImmutableDictionary<object, AnalyzerReference> CreateProjectAnalyzerReferencesMap(Project project)
        {
            return CreateAnalyzerReferencesMap(project.AnalyzerReferences.Where(reference => !_hostAnalyzerReferencesMap.Value.ContainsKey(reference.Id)));
        }

        private ImmutableDictionary<object, ImmutableArray<DiagnosticDescriptor>> CreateDiagnosticDescriptorsPerReference(
            ImmutableDictionary<object, ImmutableArray<DiagnosticAnalyzer>> analyzersMap)
        {
            var builder = ImmutableDictionary.CreateBuilder<object, ImmutableArray<DiagnosticDescriptor>>();
            foreach (var kv in analyzersMap)
            {
                var referenceId = kv.Key;
                var analyzers = kv.Value;

                var descriptors = ImmutableArray.CreateBuilder<DiagnosticDescriptor>();
                foreach (var analyzer in analyzers)
                {
                    // given map should be in good shape. no duplication. no null and etc
                    descriptors.AddRange(GetDiagnosticDescriptors(analyzer));
                }

                // there can't be duplication since _hostAnalyzerReferenceMap is already de-duplicated.
                builder.Add(referenceId, descriptors.ToImmutable());
            }

            return builder.ToImmutable();
        }

        private ImmutableDictionary<object, ImmutableArray<DiagnosticAnalyzer>> CreateHostDiagnosticAnalyzersAndBuildMap(string language)
        {
            Contract.ThrowIfNull(language);

            var nameMap = CreateAnalyzerPathToPackageNameMap();

            var builder = ImmutableDictionary.CreateBuilder<object, ImmutableArray<DiagnosticAnalyzer>>();
            foreach (var (referenceIdentity, reference) in _hostAnalyzerReferencesMap.Value)
            {
                var analyzers = reference.GetAnalyzers(language);
                if (analyzers.Length == 0)
                {
                    continue;
                }

                UpdateCompilerAnalyzerMapIfNeeded(language, analyzers);

                UpdateDiagnosticAnalyzerToPackageNameMap(nameMap, reference, analyzers);

                // there can't be duplication since _hostAnalyzerReferenceMap is already de-duplicated.
                builder.Add(referenceIdentity, analyzers);
            }

            return builder.ToImmutable();
        }

        private void UpdateDiagnosticAnalyzerToPackageNameMap(
            ImmutableDictionary<string, string> nameMap,
            AnalyzerReference reference,
            ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            if (!(reference is AnalyzerFileReference fileReference))
            {
                return;
            }

            if (!nameMap.TryGetValue(fileReference.FullPath, out var name))
            {
                return;
            }

            foreach (var analyzer in analyzers)
            {
                ImmutableInterlocked.GetOrAdd(ref _hostDiagnosticAnalyzerPackageNameMap, analyzer, name);
            }
        }

        private ImmutableDictionary<string, string> CreateAnalyzerPathToPackageNameMap()
        {
            var builder = ImmutableDictionary.CreateBuilder<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var package in _hostDiagnosticAnalyzerPackages.Value)
            {
                if (string.IsNullOrEmpty(package.Name))
                {
                    continue;
                }

                foreach (var assembly in package.Assemblies)
                {
                    if (!builder.ContainsKey(assembly))
                    {
                        builder.Add(assembly, package.Name);
                    }
                }
            }

            return builder.ToImmutable();
        }

        private void UpdateCompilerAnalyzerMapIfNeeded(string language, ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            if (_compilerDiagnosticAnalyzerMap.ContainsKey(language))
            {
                return;
            }

            foreach (var analyzer in analyzers)
            {
                if (analyzer.IsCompilerAnalyzer())
                {
                    ImmutableInterlocked.GetOrAdd(ref _compilerDiagnosticAnalyzerDescriptorMap, analyzer, a => new HashSet<string>(GetDiagnosticDescriptors(a).Select(d => d.Id)));
                    ImmutableInterlocked.GetOrAdd(ref _compilerDiagnosticAnalyzerMap, language, analyzer);
                    return;
                }
            }
        }

        private static ImmutableDictionary<object, ImmutableArray<DiagnosticAnalyzer>> CreateDiagnosticAnalyzersPerReferenceMap(
            IDictionary<object, AnalyzerReference> analyzerReferencesMap, string? language = null)
        {
            var builder = ImmutableDictionary.CreateBuilder<object, ImmutableArray<DiagnosticAnalyzer>>();

            foreach (var reference in analyzerReferencesMap)
            {
                var analyzers = language == null ? reference.Value.GetAnalyzersForAllLanguages() : reference.Value.GetAnalyzers(language);
                if (analyzers.Length == 0)
                {
                    continue;
                }

                // input "analyzerReferencesMap" is a dictionary, so there will be no duplication here.
                builder.Add(reference.Key, analyzers.WhereNotNull().ToImmutableArray());
            }

            return builder.ToImmutable();
        }

        private static ImmutableDictionary<object, AnalyzerReference> CreateAnalyzerReferencesMap(IEnumerable<AnalyzerReference> analyzerReferences)
        {
            var builder = ImmutableDictionary.CreateBuilder<object, AnalyzerReference>();
            foreach (var reference in analyzerReferences)
            {
                var key = reference.Id;

                // filter out duplicated analyzer reference
                if (builder.ContainsKey(key))
                {
                    continue;
                }

                builder.Add(key, reference);
            }

            return builder.ToImmutable();
        }

        private static ImmutableArray<AnalyzerReference> CreateAnalyzerReferencesFromPackages(
            ImmutableArray<HostDiagnosticAnalyzerPackage> analyzerPackages,
            HostAnalyzerReferenceDiagnosticReporter reporter,
            IAnalyzerAssemblyLoader? hostAnalyzerAssemblyLoader)
        {
            if (analyzerPackages.IsEmpty)
            {
                return ImmutableArray<AnalyzerReference>.Empty;
            }

            Contract.ThrowIfNull(hostAnalyzerAssemblyLoader);

            var analyzerAssemblies = analyzerPackages.SelectMany(p => p.Assemblies);

            var builder = ImmutableArray.CreateBuilder<AnalyzerReference>();
            foreach (var analyzerAssembly in analyzerAssemblies.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var reference = new AnalyzerFileReference(analyzerAssembly, hostAnalyzerAssemblyLoader);
                reference.AnalyzerLoadFailed += reporter.OnAnalyzerLoadFailed;

                builder.Add(reference);
            }

            var references = builder.ToImmutable();
            DiagnosticAnalyzerLogger.LogWorkspaceAnalyzers(references);

            return references;
        }

        private static ImmutableDictionary<object, ImmutableArray<DiagnosticAnalyzer>> MergeDiagnosticAnalyzerMap(
            ImmutableDictionary<object, ImmutableArray<DiagnosticAnalyzer>> map1, ImmutableDictionary<object, ImmutableArray<DiagnosticAnalyzer>> map2)
        {
            var current = map1;
            var seen = new HashSet<DiagnosticAnalyzer>(map1.Values.SelectMany(v => v));

            foreach (var kv in map2)
            {
                var referenceIdentity = kv.Key;
                var analyzers = kv.Value;

                if (map1.ContainsKey(referenceIdentity))
                {
                    continue;
                }

                current = current.Add(referenceIdentity, analyzers.Where(a => seen.Add(a)).ToImmutableArray());
            }

            return current;
        }

        private class HostAnalyzerReferenceDiagnosticReporter
        {
            private readonly AbstractHostDiagnosticUpdateSource? _hostUpdateSource;
            private readonly PrimaryWorkspace _primaryWorkspace;

            public HostAnalyzerReferenceDiagnosticReporter(AbstractHostDiagnosticUpdateSource? hostUpdateSource, PrimaryWorkspace primaryWorkspace)
            {
                _hostUpdateSource = hostUpdateSource;
                _primaryWorkspace = primaryWorkspace;
            }

            public void OnAnalyzerLoadFailed(object sender, AnalyzerLoadFailureEventArgs e)
            {
                if (!(sender is AnalyzerFileReference reference))
                {
                    return;
                }

                var diagnostic = AnalyzerHelper.CreateAnalyzerLoadFailureDiagnostic(e, reference.FullPath, projectId: null, language: null);

                // diagnostic from host analyzer can never go away
                var args = DiagnosticsUpdatedArgs.DiagnosticsCreated(
                    id: Tuple.Create(this, reference.FullPath, e.ErrorCode, e.TypeName),
                    workspace: _primaryWorkspace.Workspace,
                    solution: null,
                    projectId: null,
                    documentId: null,
                    diagnostics: ImmutableArray.Create<DiagnosticData>(diagnostic));

                // this can be null in test. but in product code, this should never be null.
                _hostUpdateSource?.RaiseDiagnosticsUpdated(args);
            }
        }
    }
}
