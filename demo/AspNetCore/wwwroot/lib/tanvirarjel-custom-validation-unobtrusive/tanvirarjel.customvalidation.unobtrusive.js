// Unobtrusive validation support library for TanvirArjel.CustomValidation library
// Copyright (c) TanvirArjel. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.
// @version v1.0.1

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

    var getIsoDate = function (dateString) {
        var parts = dateString.split('-');
        var day = parts[0];
        var monthName = parts[1];
        var year = parts[2];

        const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        var month = monthNames.indexOf(monthName) + 1;
        var date = year + "-" + month + "-" + day;
        return date;
    }

    var isDate = function (date) {
        if (date.length === 11) {
            date = getIsoDate(date);
        }
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
                if (stringDate.length === 11) {
                    stringDate = getIsoDate(stringDate);
                }
                return new Date(stringDate);
            }
        } else {
            throw { name: "InvalidDateTimeFormat", message: "Input date/datetime is not in valid format. Please enter the date in valid datetime format. Prefer: '01-Jan-1999' format." };
        }
    }

    // valid date validation
    $.validator.addMethod("valid-date-format", function (value, element, params) {
        if (value) {
            if (value.length === 11) {
                date = getIsoDate(value);
            }
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
        var compareProperty = $(element).closest('form').find('[name="' + comparePropertyName + '"]');
        var comparePropertyType = compareProperty.prop('type');

        return inputPropertyType === comparePropertyType;
    });

    $.validator.unobtrusive.adapters.add("input-type-compare", ['property'], function (options) {
        options.rules["input-type-compare"] = options.params;
        options.messages["input-type-compare"] = options.message;
    });

    // equality comparison validation

    $.validator.addMethod("comparison-equal", function (value, element, params) {

        const inputValue = value;
        const inputPropertyType = $(element).prop('type');

        const comparePropertyName = params.property;
        const compareProperty = $(element).closest('form').find('[name="' + comparePropertyName + '"]');
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

            if (inputPropertyType === "time" && inputPropertyType == comparePropertyType) {
                return inputValue === comparePropertyValue;
            }
        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("comparison-equal", ['property'], function (options) {
        options.rules["comparison-equal"] = options.params;
        options.messages["comparison-equal"] = options.message;
    });


    // Not equality comparison validation

    $.validator.addMethod("comparison-not-equal", function (value, element, params) {

        const inputValue = value;
        const inputPropertyType = $(element).prop('type');

        const comparePropertyName = params.property;
        const compareProperty = $(element).closest('form').find('[name="' + comparePropertyName + '"]');
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

            if (inputPropertyType === "time" && inputPropertyType == comparePropertyType) {
                return inputValue !== comparePropertyValue;
            }
        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("comparison-not-equal", ['property'], function (options) {
        options.rules["comparison-not-equal"] = options.params;
        options.messages["comparison-not-equal"] = options.message;
    });

    // greaterThan comparison validation

    $.validator.addMethod("comparison-greater-than", function (value, element, params) {
        const inputValue = value;
        let inputPropertyType = $(element).prop('type');

        const comparePropertyName = params.property;
        const compareProperty = $(element).closest('form').find('[name="' + comparePropertyName + '"]');
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

            if (inputPropertyType === "time" && inputPropertyType == comparePropertyType) {
                return inputValue > comparePropertyValue;
            }


        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("comparison-greater-than", ['property'], function (options) {
        options.rules["comparison-greater-than"] = options.params;
        options.messages["comparison-greater-than"] = options.message;
    });

    // greaterThan or equal comparison validation

    $.validator.addMethod("comparison-greater-than-or-equal", function (value, element, params) {
        const inputValue = value;
        let inputPropertyType = $(element).prop('type');

        const comparePropertyName = params.property;
        const compareProperty = $(element).closest('form').find('[name="' + comparePropertyName + '"]');
        const comparePropertyType = compareProperty.prop('type');
        const comparePropertyValue = compareProperty.val();

        if (inputValue && comparePropertyValue) {

            if (inputPropertyType === "number" && inputPropertyType === comparePropertyType) {
                return Number(inputValue) >= Number(comparePropertyValue);
            }

            if (inputPropertyType === "text" && inputPropertyType === comparePropertyType) {
                if (isDate(inputValue) && isDate(comparePropertyValue)) {
                    const inputDate = getDateValue(inputValue);
                    const compareDate = getDateValue(comparePropertyValue);
                    return inputDate >= compareDate;
                } else {
                    return inputValue.length >= comparePropertyValue.length;
                }
            }

            if (inputPropertyType.indexOf("date") !== -1 && comparePropertyType.indexOf("date") !== -1) {
                const inputDate = getDateValue(value);
                const compareDate = getDateValue(comparePropertyValue);
                return inputDate >= compareDate;
            }

            if (inputPropertyType === "time" && inputPropertyType == comparePropertyType) {
                return inputValue >= comparePropertyValue;
            }
        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("comparison-greater-than-or-equal", ['property'], function (options) {
        options.rules["comparison-greater-than-or-equal"] = options.params;
        options.messages["comparison-greater-than-or-equal"] = options.message;
    });

    // smallerThan comparison validation

    $.validator.addMethod("comparison-smaller-than", function (value, element, params) {
        const inputValue = value;
        const inputPropertyType = $(element).prop('type');

        const comparePropertyName = params.property;
        const compareProperty = $(element).closest('form').find('[name="' + comparePropertyName + '"]');
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

            if (inputPropertyType === "time" && inputPropertyType == comparePropertyType) {
                return inputValue < comparePropertyValue;
            }
        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("comparison-smaller-than", ['property'], function (options) {
        options.rules["comparison-smaller-than"] = options.params;
        options.messages["comparison-smaller-than"] = options.message;
    });

    // smallerThan or equal comparison validation

    $.validator.addMethod("comparison-smaller-than-or-equal", function (value, element, params) {
        const inputValue = value;
        const inputPropertyType = $(element).prop('type');

        const comparePropertyName = params.property;
        const compareProperty = $(element).closest('form').find('[name="' + comparePropertyName + '"]');
        const comparePropertyType = compareProperty.prop('type');
        const comparePropertyValue = compareProperty.val();

        if (inputValue && comparePropertyValue) {

            if (inputPropertyType === "number" && inputPropertyType === comparePropertyType) {
                return Number(inputValue) <= Number(comparePropertyValue);
            }

            if (inputPropertyType === "text" && inputPropertyType === comparePropertyType) {
                if (isDate(inputValue) && isDate(comparePropertyValue)) {
                    const inputDate = getDateValue(inputValue);
                    const compareDate = getDateValue(comparePropertyValue);
                    return inputDate <= compareDate;
                } else {
                    return inputValue.length <= comparePropertyValue.length;
                }
            }

            if (inputPropertyType.indexOf("date") !== -1 && comparePropertyType.indexOf("date") !== -1) {
                const inputDate = getDateValue(value);
                const compareDate = getDateValue(comparePropertyValue);
                return inputDate <= compareDate;
            }

            if (inputPropertyType === "time" && inputPropertyType == comparePropertyType) {
                return inputValue <= comparePropertyValue;
            }
        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("comparison-smaller-than-or-equal", ['property'], function (options) {
        options.rules["comparison-smaller-than-or-equal"] = options.params;
        options.messages["comparison-smaller-than-or-equal"] = options.message;
    });

    // text editor required validation

    $.validator.addMethod("texteditor-required", function (value, element, params) {
        const inputValueWithoutHtml = jQuery($("<p>").html(value)).text().replace(/\s\s+/g, ' ');
        return inputValueWithoutHtml !== " " && inputValueWithoutHtml.length > 0;
    });

    $.validator.unobtrusive.adapters.add("texteditor-required", [], function (options) {
        options.rules["texteditor-required"] = options.params;
        options.messages["texteditor-required"] = options.message;
    });

    // text editor minlength validation

    $.validator.addMethod("texteditor-minlength", function (value, element, params) {
        const inputValueWithoutHtml = jQuery($("<p>").html(value)).text().replace(/\s\s+/g, ' ');
        const minLength = params.value;
        return inputValueWithoutHtml !== " " && inputValueWithoutHtml.length >= minLength;
    });

    $.validator.unobtrusive.adapters.add("texteditor-minlength", ['value'], function (options) {
        options.rules["texteditor-minlength"] = options.params;
        options.messages["texteditor-minlength"] = options.message;
    });

    // text editor minlength validation

    $.validator.addMethod("texteditor-maxlength", function (value, element, params) {
        const inputValueWithoutHtml = jQuery($("<p>").html(value)).text().replace(/\s\s+/g, ' ');
        const maxLength = params.value;
        return inputValueWithoutHtml !== " " && inputValueWithoutHtml.length <= maxLength;
    });

    $.validator.unobtrusive.adapters.add("texteditor-maxlength", ['value'], function (options) {
        options.rules["texteditor-maxlength"] = options.params;
        options.messages["texteditor-maxlength"] = options.message;
    });

    // FixedLength validation

    $.validator.addMethod("fixed-length", function (value, element, params) {
        const inputValueWithoutHtml = jQuery($("<p>").html(value)).text().replace(/\s\s+/g, ' ');
        const fixedLength = params.value;

        if (inputValueWithoutHtml) {
            return inputValueWithoutHtml.length == fixedLength;
        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("fixed-length", ['value'], function (options) {
        options.rules["fixed-length"] = options.params;
        options.messages["fixed-length"] = options.message;
    });

    // RequiredIf validation

    $.validator.addMethod("requiredif", function (value, element, params) {
        const otherPropertyName = params['other-property'];
        const comparisonType = params['comparison-type'];
        const otherPropertyType = params['other-property-type'];
        let otherPropertyValue = params['other-property-value'];


        const otherPropertyElement = $(element).closest('form').find('[name="' + otherPropertyName + '"]');
        const otherPropertyInputType = otherPropertyElement.attr('type');

        let otherPropertyCurrentValue = null;

        if (otherPropertyInputType == "checkbox" || otherPropertyInputType == "radio") {
            var control = $("[name$='" + otherPropertyName + "']:checked");
            otherPropertyCurrentValue = control.val();
        } else {
            otherPropertyCurrentValue = otherPropertyElement.val();
        }

        if (otherPropertyType == "number") {
            otherPropertyValue = Number(otherPropertyValue);
            otherPropertyCurrentValue = Number(otherPropertyCurrentValue);
        } else if (otherPropertyType == "datetime") {
            if (otherPropertyValue) {
                otherPropertyValue = getDateValue(otherPropertyValue);
                if (comparisonType == "Equal" || comparisonType == "NotEqual") {
                    otherPropertyValue = otherPropertyValue.getTime();
                } 
            }

            if (otherPropertyCurrentValue) {
                otherPropertyCurrentValue = getDateValue(otherPropertyCurrentValue);
                if (comparisonType == "Equal" || comparisonType == "NotEqual") {
                    otherPropertyCurrentValue = otherPropertyCurrentValue.getTime();
                } 
            }
        } else if (otherPropertyType == "string") {
            if (comparisonType == "Equal" || comparisonType == "NotEqual") {
                otherPropertyValue = otherPropertyValue;
                otherPropertyCurrentValue = otherPropertyCurrentValue;
            } else {
                otherPropertyCurrentValue = otherPropertyCurrentValue.length;
                otherPropertyValue = otherPropertyValue.length;
            }
            
        } else if (otherPropertyType == "timespan") {
            otherPropertyValue = otherPropertyValue;
            otherPropertyCurrentValue = otherPropertyCurrentValue;
        } else {
            throw { name: "InvalidTypeException", message: "The type in not supported in requiredif validation." };
        }

        if (comparisonType == "Equal") {
            if (otherPropertyCurrentValue == otherPropertyValue) {
                if (value) {
                    return true;
                }
                return false;
            }
        } else if (comparisonType == "NotEqual") {
            if (otherPropertyCurrentValue != otherPropertyValue) {
                if (value) {
                    return true;
                }
                return false;
            }
        } else if (comparisonType == "GreaterThan") {
            if (otherPropertyCurrentValue > otherPropertyValue) {
                if (value) {
                    return true;
                }
                return false;
            }
        } else if (comparisonType == "GreaterThanOrEqual") {
            if (otherPropertyCurrentValue >= otherPropertyValue) {
                if (value) {
                    return true;
                }
                return false;
            }
        } else if (comparisonType == "SmallerThan") {
            if (otherPropertyCurrentValue < otherPropertyValue) {
                if (value) {
                    return true;
                }
                return false;
            }
        } else if (comparisonType == "SmallerThanOrEqual") {
            if (otherPropertyCurrentValue <= otherPropertyValue) {
                if (value) {
                    return true;
                }
                return false;
            }
        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("requiredif", ['other-property', 'comparison-type', 'other-property-type', 'other-property-value'], function (options) {
        options.rules["requiredif"] = options.params;
        options.messages["requiredif"] = options.message;
    });

}(jQuery));