(function () {
    'use strict';

    var mod = angular.module('ngValidation', []);

    mod.value('$jqVal', jQuery.validator);

    // Service to retrieve validation metedata for .NET types
    mod.service('validationMetadata', ['$http', '$q', function ($http, $q) {
        var svc = {
            loadMetaDataForType: loadMetaDataForType
        };

        var typeMetaDataCache = {};

        return svc;

        function loadMetaDataForType(typeName) {
            var cachedValue = typeMetaDataCache[typeName];
            if (angular.isUndefined(cachedValue)) {
                // new requested type
                var promise = $http.get('/ngval/TypeMetaData/?typeName=' + encodeURIComponent(typeName)).then(function (result) {
                    typeMetaDataCache[typeName] = result.data;
                    return result.data;
                });

                // store the promise in the cache, so other requests for the same can listen to the same promise
                typeMetaDataCache[typeName] = promise;

                return promise;

            } else if (cachedValue.then && angular.isFunction(cachedValue.then)) {
                // other request for same type already in progress - return the promise
                return cachedValue;
            } else {
                // already got the value
                var deferred = $q.defer();
                deferred.resolve(cachedValue);
                return deferred.promise;
            }
        }
    }]);

    // Directive to specify the model type to validate - apply to form
    mod.directive('ngvalType', ['$jqVal', 'validationMetadata', function ($jqVal, validationMetadata) {
        return {
            scope: true,
            restrict: 'A',
            compile: function compile(tElement, tAttrs, transclude) {
                return {
                    pre: function preLink(scope, elem, attrs) {
                        var form = elem[0];
                        if (form.tagName.toLowerCase() === "form") {

                            var modelType = attrs['ngvalType'];
                            scope.ngval = {
                                modelType: modelType,
                                fieldAdapters: {},
                                errorMessages: {},
                                form: scope[attrs.name],
                                validate: function () {
                                    var theForm = this.form;
                                    angular.forEach(this.fieldAdapters, function (field, fieldName) {
                                        var field = theForm[fieldName];
                                        if (!angular.isUndefined(field)) {
                                            field.$validate();
                                        }
                                    });

                                    return this.form.$valid;
                                }
                            };

                            validationMetadata.loadMetaDataForType(modelType).then(function (metadata) {
                                var globalRules = {};
                                var globalMessages = {};
                                metadata.Fields.forEach(function (fieldMetadata) {
                                    // the field to validate
                                    var fieldName = fieldMetadata.FieldName;
                                    var fieldAdapters = [];

                                    var rules = globalRules[fieldName] = {};
                                    var messages = globalMessages[fieldName] = {};

                                    // loop through the rules and set up the adapterLink - this prepares the objct we will pass to adapt, all but the element itself
                                    fieldMetadata.ValidationRules.forEach(function (validationRule) {

                                        var adapters = $jqVal.unobtrusive.adapters.filter(function (a) { return a.name == validationRule.ValidationType; });
                                        if (adapters.length) {
                                            fieldAdapters.push({
                                                adapter: adapters[0],
                                                adaptParams: {
                                                    element: null, // fill this in in ngval
                                                    form: form,
                                                    message: validationRule.ErrorMessage,
                                                    params: validationRule.ValidationParameters,
                                                    rules: rules,
                                                    messages: messages
                                                }
                                            });
                                        } else {
                                            console.error('adapter', validationRule.ValidationType, 'does not exist');
                                        }
                                    });

                                    scope.ngval.fieldAdapters[fieldName] = {
                                        apply: function (fieldElement) {
                                            fieldAdapters.forEach(function (fieldAdapter) {
                                                fieldAdapter.adaptParams.element = fieldElement;
                                                fieldAdapter.adapter.adapt(fieldAdapter.adaptParams);
                                            });
                                        }
                                    }
                                });

                                scope.$broadcast('ngvalLoaded', scope.ngval);

                                // attach validation
                                var options = {  // options structure passed to jQuery Validate's validate() method
                                    //errorClass: defaultOptions.errorClass || "input-validation-error",
                                    //errorElement: defaultOptions.errorElement || "span",
                                    //errorClass: "input-validation-error",
                                    errorElement: "span",
                                    showErrors: function () {
                                        //debugger;
                                    },
                                    errorPlacement: function (error, element) {
                                        //debugger;
                                        return false;
                                        //onError.apply(form, arguments);
                                        //execInContext("errorPlacement", arguments);
                                    },
                                    invalidHandler: function () {
                                        //debugger;
                                        //    //onErrors.apply(form, arguments);
                                        //    //execInContext("invalidHandler", arguments);
                                    },
                                    messages: globalMessages,
                                    rules: globalRules,
                                    success: function () {
                                        //onSuccess.apply(form, arguments);
                                        //execInContext("success", arguments);
                                    }
                                };

                                $.data(elem[0], "validator", null);

                                scope.ngval.validator = elem.validate(options);

                            });
                        }
                    },
                    post: angular.noop
                }
            }
        };
    }]);

    // The validator directive, which calls MVC validation - apply to a field (input, select)
    mod.directive('ngval', function ($timeout) {
        return {
            scope: {},
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, elem, attrs, ngModelController) {
                var fieldName = attrs.name;

                scope.$on('ngvalLoaded', function (event, ngval) {

                    ngval.fieldAdapters[fieldName].apply(elem[0]);

                    // attach custom validator to the element
                    ngModelController.$validators.ngval = function (modelValue, viewValue) {

                        var validator = ngval.validator;
                        var isValid = validator.element(elem);
                        if (isValid) {
                            ngval.errorMessages[fieldName] = null;
                        } else {
                            ngval.errorMessages[fieldName] = validator.errorList[0].message;
                        }

                        return isValid;
                    };

                    //$timeout(ngModelController.$validate, 0);
                });
            }
        }
    });

    // Directive to show the error messsage for a field
    mod.directive('ngvalMessage', function () {
        return {
            restrict: 'A',
            scope: {},
            link: function (scope, elm, attrs, ctrl) {
                //debugger;
                scope.fieldName = attrs['ngvalMessage'];

                scope.$on('ngvalLoaded', function (event, ngval) {
                    scope.ngval = ngval;
                    scope.form = ngval.form;
                });
            },
            replace: true,
            template: '<span ng-show="form[fieldName].$error.ngval">{{ngval.errorMessages[fieldName]}}</span>'
        }
    });
})();