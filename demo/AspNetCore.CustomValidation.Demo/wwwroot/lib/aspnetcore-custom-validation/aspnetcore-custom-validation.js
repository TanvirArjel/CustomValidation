// Unobtrusive validation support library for AspNetCore.CustomValidation library
// Copyright (c) TanvirArjel. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.
// @version v1.1.0

(function ($) {

    // set defaults for all forms on this page
    $.validator.setDefaults({
        onfocusout: function (element, event) {
            if (event.which === 9 && this.elementValue(element) === "") {
                return;
            } else {
                this.element(element);
            }
        }
    });

    var isDate = function (date) {
        return new Date(date).toString() !== "Invalid Date" && !isNaN(new Date(date));
    }

    var isIsoDate = function (inputString) {
        const regex = /\d{4}-\d{2}-\d{2}/i;
        return regex.test(inputString);
    }

    var getDateValue = function (stringDate) {
        if (isDate(stringDate)) {
            if (isIsoDate(stringDate)) {
                const dateValue = stringDate.indexOf("T") === -1 ? new Date(stringDate + "T00:00:00") : new Date(stringDate);
                return dateValue;
            } else {
                return new Date(stringDate);
            }
        } else {
            throw { name: "InvalidDateTimeFormat", message: "Input date/datetime is not in valid format. Please enter the date in valid datetime format. Prefer: '01-Jan-1999' format." };
        }
    }

    // min date validation
    $.validator.addMethod("valid-date-format", function (value, element, params) {
        if (value) {
            return isDate(value);
        }
        return true;
    });

    $.validator.unobtrusive.adapters.add("valid-date-format", [], function (options) {
        options.rules["valid-date-format"] = {};
        options.messages["valid-date-format"] = options.message;
    });


    // min date validation
    $.validator.addMethod("mindate", function (value, element, params) {
        if (value) {
            const minDate = new Date(params.date);
            const inputDate = getDateValue(value);
            return inputDate >= minDate;
        }
        return true;
    });

    $.validator.unobtrusive.adapters.add("mindate", ['date'], function (options) {
        options.rules.mindate = options.params;
        options.messages["mindate"] = options.message;
    });

    // max date validation
    $.validator.addMethod("maxdate", function (value, element, params) {
        if (value) {
            const maxDate = new Date(params.date);
            const inputDate = getDateValue(value);
            return inputDate <= maxDate;
        }
        return true;
    });

    $.validator.unobtrusive.adapters.add("maxdate", ['date'], function (options) {
        options.rules.maxdate = options.params;
        options.messages["maxdate"] = options.message;
    });

    // must be smaller than current time validation
    $.validator.addMethod("currenttime", function (value, element, params) {
        if (value) {
            const currentTime = new Date();
            const inputDate = getDateValue(value);
            return currentTime > inputDate;
        }
        return true;
    });

    $.validator.unobtrusive.adapters.add("currenttime", [], function (options) {
        options.rules.currenttime = {};
        options.messages["currenttime"] = options.message;
    });

    // max age validation
    $.validator.addMethod("maxage", function (value, element, params) {
        if (value) {
            let maxAgeDateTime = new Date();

            maxAgeDateTime.setFullYear(maxAgeDateTime.getFullYear() - params.years);
            maxAgeDateTime.setMonth(maxAgeDateTime.getMonth() - params.months);
            maxAgeDateTime.setDate(maxAgeDateTime.getDate() - params.days);

            const inputDate = getDateValue(value);
            return inputDate >= maxAgeDateTime;
        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("maxage", ['years', 'months', 'days'], function (options) {
        options.rules.maxage = options.params;
        options.messages["maxage"] = options.message;
    });

    // min age validation
    $.validator.addMethod("minage", function (value, element, params) {
        if (value) {
            let minAgeDateTime = new Date();
            minAgeDateTime.setFullYear(minAgeDateTime.getFullYear() - params.years);
            minAgeDateTime.setMonth(minAgeDateTime.getMonth() - params.months);
            minAgeDateTime.setDate(minAgeDateTime.getDate() - params.days);

            const inputDate = getDateValue(value);
            return minAgeDateTime >= inputDate;
        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("minage", ['years', 'months', 'days'], function (options) {
        options.rules.minage = options.params;
        options.messages["minage"] = options.message;
    });

    // filetype validation

    $.validator.addMethod("filetype", function (value, element, params) {
        var selectedFileType = value.split('.').pop();
        var validFileTypes = params.validtypes;
        return validFileTypes.toLowerCase().indexOf(selectedFileType.toLowerCase()) !== -1;
    });

    $.validator.unobtrusive.adapters.add("filetype", ['validtypes'], function (options) {
        options.rules.filetype = options.params;
        options.messages["filetype"] = options.message;
    });

    // file minsize validation

    $.validator.addMethod("file-minsize", function (value, element, params) {
        if (value && element.files[0]) {
            var allowedMinSize = params.value;
            var selectedFileSizeInKByte = element.files[0].size / 1024;
            return selectedFileSizeInKByte >= allowedMinSize;
        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("file-minsize", ['value'], function (options) {
        options.rules["file-minsize"] = options.params;
        options.messages["file-minsize"] = options.message;
    });

    // file maxsize validation

    $.validator.addMethod("file-maxsize", function (value, element, params) {
        if (value && element.files[0]) {
            var allowedMinSize = params.value;
            var selectedFileSizeInKByte = element.files[0].size / 1024;
            return selectedFileSizeInKByte <= allowedMinSize;
        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("file-maxsize", ['value'], function (options) {
        options.rules["file-maxsize"] = options.params;
        options.messages["file-maxsize"] = options.message;
    });

    // input type validation validation

    $.validator.addMethod("input-type-compare", function (value, element, params) {
        var inputPropertyType = $(element).prop('type');
        var comparePropertyName = params["property"];
        var compareProperty = $(element).closest('form').find('input[name="' + comparePropertyName + '"]');
        var comparePropertyType = compareProperty.prop('type');

        return inputPropertyType === comparePropertyType;
    });

    $.validator.unobtrusive.adapters.add("input-type-compare", ['property'], function (options) {
        options.rules["input-type-compare"] = options.params;
        options.messages["input-type-compare"] = options.message;
    });

    // equality comparison validation

    $.validator.addMethod("comparison-equality", function (value, element, params) {

        const inputValue = value;
        const inputPropertyType = $(element).prop('type');

        const comparePropertyName = params.property;
        const compareProperty = $(element).closest('form').find('input[name="' + comparePropertyName + '"]');
        const comparePropertyType = compareProperty.prop('type');
        const comparePropertyValue = compareProperty.val();

        if (inputValue && comparePropertyValue) {

            if (inputPropertyType === "number" && inputPropertyType === comparePropertyType) {
                return Number(value) === Number(compareProperty.val());
            }

            if (inputPropertyType === "text" && inputPropertyType === comparePropertyType) {
                if (isDate(inputValue) && isDate(comparePropertyValue)) {
                    const inputDate = getDateValue(inputValue);
                    const compareDate = getDateValue(comparePropertyValue);
                    return inputDate.getTime() === compareDate.getTime();
                } else {
                    return inputValue.length === comparePropertyValue.length;
                }
            }

            if (inputPropertyType.indexOf("date") !== -1 && comparePropertyType.indexOf("date") !== -1) {
                const inputDate = getDateValue(value);
                const compareDate = getDateValue(comparePropertyValue);
                return inputDate.getTime() === compareDate.getTime();
            }
        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("comparison-equality", ['property'], function (options) {
        options.rules["comparison-equality"] = options.params;
        options.messages["comparison-equality"] = options.message;
    });


    // Not equality comparison validation

    $.validator.addMethod("comparison-not-equality", function (value, element, params) {

        const inputValue = value;
        const inputPropertyType = $(element).prop('type');

        const comparePropertyName = params.property;
        const compareProperty = $(element).closest('form').find('input[name="' + comparePropertyName + '"]');
        const comparePropertyType = compareProperty.prop('type');
        const comparePropertyValue = compareProperty.val();

        if (inputValue && comparePropertyValue) {

            if (inputPropertyType === "number" && inputPropertyType === comparePropertyType) {
                return Number(value) !== Number(compareProperty.val());
            }

            if (inputPropertyType === "text" && inputPropertyType === comparePropertyType) {
                if (isDate(inputValue) && isDate(comparePropertyValue)) {
                    const inputDate = getDateValue(inputValue);
                    const compareDate = getDateValue(comparePropertyValue);
                    return inputDate.getTime() !== compareDate.getTime();
                } else {
                    return inputValue.length !== comparePropertyValue.length;
                }
            }

            if (inputPropertyType.indexOf("date") !== -1 && comparePropertyType.indexOf("date") !== -1) {
                const inputDate = getDateValue(value);
                const compareDate = getDateValue(comparePropertyValue);
                return inputDate.getTime() !== compareDate.getTime();
            }
        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("comparison-not-equality", ['property'], function (options) {
        options.rules["comparison-not-equality"] = options.params;
        options.messages["comparison-not-equality"] = options.message;
    });

    // greaterThan comparison validation

    $.validator.addMethod("comparison-greater", function (value, element, params) {
        const inputValue = value;
        let inputPropertyType = $(element).prop('type');

        const comparePropertyName = params.property;
        const compareProperty = $(element).closest('form').find('input[name="' + comparePropertyName + '"]');
        const comparePropertyType = compareProperty.prop('type');
        const comparePropertyValue = compareProperty.val();

        if (inputValue && comparePropertyValue) {

            if (inputPropertyType === "number" && inputPropertyType === comparePropertyType) {
                return Number(inputValue) > Number(comparePropertyValue);
            }

            if (inputPropertyType === "text" && inputPropertyType === comparePropertyType) {
                if (isDate(inputValue) && isDate(comparePropertyValue)) {
                    const inputDate = getDateValue(inputValue);
                    const compareDate = getDateValue(comparePropertyValue);
                    return inputDate > compareDate;
                } else {
                    return inputValue.length > comparePropertyValue.length;
                }
            }

            if (inputPropertyType.indexOf("date") !== -1 && comparePropertyType.indexOf("date") !== -1) {
                const inputDate = getDateValue(value);
                const compareDate = getDateValue(comparePropertyValue);
                return inputDate > compareDate;
            }
        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("comparison-greater", ['property'], function (options) {
        options.rules["comparison-greater"] = options.params;
        options.messages["comparison-greater"] = options.message;
    });

    // smallerThan comparison validation

    $.validator.addMethod("comparison-smaller", function (value, element, params) {
        const inputValue = value;
        const inputPropertyType = $(element).prop('type');

        const comparePropertyName = params.property;
        const compareProperty = $(element).closest('form').find('input[name="' + comparePropertyName + '"]');
        const comparePropertyType = compareProperty.prop('type');
        const comparePropertyValue = compareProperty.val();

        if (inputValue && comparePropertyValue) {

            if (inputPropertyType === "number" && inputPropertyType === comparePropertyType) {
                return Number(inputValue) < Number(comparePropertyValue);
            }

            if (inputPropertyType === "text" && inputPropertyType === comparePropertyType) {
                if (isDate(inputValue) && isDate(comparePropertyValue)) {
                    const inputDate = getDateValue(inputValue);
                    const compareDate = getDateValue(comparePropertyValue);
                    return inputDate < compareDate;
                } else {
                    return inputValue.length < comparePropertyValue.length;
                }
            }

            if (inputPropertyType.indexOf("date") !== -1 && comparePropertyType.indexOf("date") !== -1) {
                const inputDate = getDateValue(value);
                const compareDate = getDateValue(comparePropertyValue);
                return inputDate < compareDate;
            }
        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("comparison-smaller", ['property'], function (options) {
        options.rules["comparison-smaller"] = options.params;
        options.messages["comparison-smaller"] = options.message;
    });

    // tinymce required validation

    $.validator.addMethod("tinymce-required", function (value, element, params) {
        const inputValueWithoutHtml = jQuery($("<p>").html(value)).text().replace(/\s\s+/g, ' ');
        return inputValueWithoutHtml !== " " && inputValueWithoutHtml.length > 0;
    });

    $.validator.unobtrusive.adapters.add("tinymce-required", [], function (options) {
        options.rules["tinymce-required"] = options.params;
        options.messages["tinymce-required"] = options.message;
    });

    // tinymce minlength validation

    $.validator.addMethod("tinymce-minlength", function (value, element, params) {
        const inputValueWithoutHtml = jQuery($("<p>").html(value)).text().replace(/\s\s+/g, ' ');
        const minLength = params.value;
        return inputValueWithoutHtml !== " " && inputValueWithoutHtml.length >= minLength;
    });

    $.validator.unobtrusive.adapters.add("tinymce-minlength", ['value'], function (options) {
        options.rules["tinymce-minlength"] = options.params;
        options.messages["tinymce-minlength"] = options.message;
    });

    // tinymce minlength validation

    $.validator.addMethod("tinymce-maxlength", function (value, element, params) {
        const inputValueWithoutHtml = jQuery($("<p>").html(value)).text().replace(/\s\s+/g, ' ');
        const maxLength = params.value;
        return inputValueWithoutHtml !== " " && inputValueWithoutHtml.length <= maxLength;
    });

    $.validator.unobtrusive.adapters.add("tinymce-maxlength", ['value'], function (options) {
        options.rules["tinymce-maxlength"] = options.params;
        options.messages["tinymce-maxlength"] = options.message;
    });

}(jQuery));