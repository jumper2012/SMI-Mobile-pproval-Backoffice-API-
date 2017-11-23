using System;
using System.Collections.Generic;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public abstract class ContentFieldValidator : IGlobalSingletonDependency {

        private static readonly IDependencyProvider _dp = ProtoEngine.GetDependencyProvider();
        private static ContentFieldValidator[] _definedValidators;

        private Type[] _handledFormTypes;

        static ContentFieldValidator() {
            ValidatorTypes = TypesCache.AppDomainTypes.Where(
                x => typeof(ContentFieldValidator).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract &&
                     !x.IsInterface && !x.IsGenericTypeDefinition
            ).ToArray();
        }

        /// <summary>
        ///     The name of this validator.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        ///     The description of this validator.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        ///     Validator config object type.
        /// </summary>
        public abstract Type ConfigType { get; }

        /// <summary>
        ///     Array of content modifier form types that can be validated by this validator.
        /// </summary>
        public Type[] HandledFormTypes {
            get {
                if (_handledFormTypes == null) {
                    var hfts = new List<Type>();
                    foreach (var t in TypesCache.AppDomainTypes.Where(x =>
                        typeof(ContentModifierForm).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract &&
                        !x.IsInterface && !x.IsGenericTypeDefinition)) {
                        if (CanValidate(t)) {
                            hfts.Add(t);
                        }
                    }
                    if (hfts.Count == 0) {
                        throw new InvalidOperationException(
                            $"ProtoCMS: content field validator '{Name}' can't validate any form type.");
                    }
                    _handledFormTypes = hfts.ToArray();
                }
                return _handledFormTypes;
            }
        }

        public static Type[] ValidatorTypes { get; }
        public static ContentFieldValidator[] DefinedValidators {
            get {
                if (_definedValidators == null) {
                    var dvs = new List<ContentFieldValidator>();
                    foreach (var prt in ValidatorTypes) {
                        var valr = _dp.GetService(prt).DirectCastTo<ContentFieldValidator>();
                        dvs.Add(valr);
                    }
                    _definedValidators = dvs.OrderBy(x => x.Name).ToArray();
                }
                return _definedValidators;
            }
        }

        /// <summary>
        ///     Whether this validator can validate a form type
        /// </summary>
        /// <param name="formType">The form type</param>
        /// <returns>True if can validate that form type</returns>
        protected abstract bool CanValidate(Type formType);

        /// <summary>
        ///     The validate function.
        /// </summary>
        /// <param name="form">The form to validate.</param>
        /// <param name="config">Validator configuration.</param>
        /// <param name="contentType">Content type that wants validation.</param>
        /// <param name="fieldDefinition">Content field definition related to this validation.</param>
        /// <returns>Validation result</returns>
        protected abstract FurtherValidationResult Validate(ContentModifierForm form,
            ContentFieldValidatorConfiguration config,
            ContentType contentType, ContentFieldDefinition fieldDefinition);

        /// <summary>
        ///     Validate the form
        /// </summary>
        public FurtherValidationResult ValidateForm(ContentModifierForm form,
            ContentFieldValidatorConfiguration config,
            ContentType contentType, ContentFieldDefinition fieldDefinition) {
            ValidateValidatorParams();
            return Validate(form, config, contentType, fieldDefinition);
        }

        private void ValidateValidatorParams() {
            if (string.IsNullOrWhiteSpace(Name)) {
                throw new InvalidOperationException($"ProtoCMS: validator name is required.");
            }
            if (!ContentType.VALID_ID_REGEX.IsMatch(Name)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: validator name must match regex '{ContentType.VALID_ID_PATTERN}'.");
            }
            if (HandledFormTypes == null || HandledFormTypes.Length == 0) {
                throw new InvalidOperationException(
                    $"ProtoCMS: validator handled form types is required.");
            }
            foreach (var hft in HandledFormTypes) {
                if (!typeof(ContentModifierForm).IsAssignableFrom(hft)) {
                    throw new InvalidOperationException($"ProtoCMS: validator's handled form type elements must all " +
                                                        $"inherit from '{typeof(ContentModifierForm).FullName}' " +
                                                        $"(problem type: '{hft}').");
                }
            }
            if (ConfigType == null) {
                throw new InvalidOperationException($"ProtoCMS: validator config type is required.");
            }
            if (!typeof(ContentFieldValidatorConfiguration).IsAssignableFrom(ConfigType)) {
                throw new InvalidOperationException($"ProtoCMS: validator config type must be an instance of " +
                                                    $"'{typeof(ContentFieldValidatorConfiguration).FullName}'.");
            }
            if (ConfigType.GetConstructor(Type.EmptyTypes) == null) {
                throw new InvalidOperationException($"ProtoCMS: validator config type must have a default " +
                                                    $"parameterless constructor.");
            }
        }
    }
}