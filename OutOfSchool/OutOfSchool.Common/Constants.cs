﻿namespace OutOfSchool.Common;

public static class Constants
{
    /// <summary>
    /// Maximum length of unified URL.
    /// </summary>
    // TODO: When we finish transition to unified contacts - change to 2048 (Max length for Chrome, which is minimal between browsers)
    public const int MaxUnifiedUrlLength = 256;

    public const int MaxEmailTypeLength = 60;

    public const int MaxEmailAddressLength = 256;

    public const long DefaultCityCodeficatorId = 31737;

    public const string DefaultAuthScheme = "bearer_or_cookie";

    public const string PhoneNumberFormat = "{0:XXXX XX-XXX-XX-XX}";

    public const string PhoneErrorMessage = "Error! Please check the number is correct";

    public const int MySQLServerMinimalMajorVersion = 8;

    public const string NameRegexViewModel = @"^[А-Яа-яҐґЄєІіЇї](([\'\-][А-Яа-яҐґЄєІіЇї])?[А-Яа-яҐґЄєІіЇї]*)*$";

    public const string NameErrorMessage = "Check the entered data. Please use only cyrillic and symbols( ' - )";

    public const string EmailRegexViewModel = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

    public const string PasswordValidationErrorMessage = "Password must be at least 8 characters long, including upper and lower case letters, digits and special characters (@$!%*?&)";

    public const string PasswordRequiredErrorMessage = "Password is required";

    public const int PasswordMinLength = 8;

    public const string ValidationSymbols = "@$!%*?&";

    public const string AddressSeparator = ", ";

    public const string EnumErrorMessage = "{0} should be in enum range";

    public const string AdminKeyword = "Admin";

    public const char MappingSeparator = '¤';

    public const int NameMaxLength = 60;

    public const int ChatMessageTextMaxLength = 256;

    public const string CacheProfilePrivate = "CacheProfilePrivate";

    public const string CacheProfilePublic = "CacheProfilePublic";

    public const string PathToChatHub = "/hubs/chat";

    public const string PathToNotificationHub = "/hubs/notification";

    public const string WorkshopNotFoundErrorMessage = "Workshop not found.";

    public const string InvalidAvailableSeatsForWorkshopErrorMessage =
        "The number of available seats must be equal or greater than the number of taken seats.";

    public const string UnknownErrorDuringUpdateMessage = "Unknown error occurred during update.";

    /// <summary>
    /// Longest possible length of provider founder.
    /// </summary>
    public const int MaxProviderFounderLength = 60;

    /// <summary>
    /// Shortest possible phone number length without '+' prefix.
    /// </summary>
    public const int MinPhoneNumberLength = 7;

    /// <summary>
    /// Longest possible phone number length without '+' prefix.
    /// </summary>
    public const int MaxPhoneNumberLength = 15;

    /// <summary>
    /// Shortest possible phone number length with '+' prefix.
    /// </summary>
    public const int MinPhoneNumberLengthWithPlusSign = MinPhoneNumberLength + 1;

    /// <summary>
    /// Longest possible phone number length with '+' prefix.
    /// </summary>
    public const int MaxPhoneNumberLengthWithPlusSign = MaxPhoneNumberLength + 1;

    /// <summary>
    /// Error message for invalid first name.
    /// </summary>
    public const string InvalidFirstNameErrorMessage = "First name contains invalid characters";

    /// <summary>
    /// Error message for invalid middle name.
    /// </summary>
    public const string InvalidMiddleNameErrorMessage = "Middle name contains invalid characters";

    /// <summary>
    /// Error message for invalid last name.
    /// </summary>
    public const string InvalidLastNameErrorMessage = "Last name contains invalid characters";

    /// <summary>
    /// Error message for required first name.
    /// </summary>
    public const string RequiredFirstNameErrorMessage = "First name is required";

    /// <summary>
    /// Error message for required middle name.
    /// </summary>
    public const string RequiredLastNameErrorMessage = "Last name is required";

    /// <summary>
    /// Ukrainian name regex pattern that allows only characters that allowed in Ukrainian name.
    /// </summary>
    public const string UkrainianNameRegexPattern = @"^[А-ЩЬЮЯҐЄІЇа-щьюяґєії0-9\'\-\ ]+$";

    /// <summary>
    /// Minimum age for user (parent, provider).
    /// </summary>
    public const int AdultAge = 18;

    /// <summary>
    /// Error message for day of birth validation.
    /// </summary>
    public const string DayOfBirthErrorMessage = "Error! Please check the day of birth is correct";

    /// <summary>
    /// Minimum length of workshop title.
    /// </summary>
    public const int MinWorkshopTitleLength = 1;

    /// <summary>
    /// Maximum length of workshop title.
    /// </summary>
    public const int MaxWorkshopTitleLength = 60;

    /// <summary>
    /// Minimum length of workshop short title.
    /// </summary>
    public const int MinWorkshopShortTitleLength = 1;

    /// <summary>
    /// Maximum length of workshop short title.
    /// </summary>
    public const int MaxWorkshopShortTitleLength = 60;

    /// <summary>
    /// Minimum length of provider full title.
    /// </summary>
    public const int MinProviderFullTitleLength = 1;

    /// <summary>
    /// Maximum length of provider full title.
    /// </summary>
    public const int MaxProviderFullTitleLength = 60;

    /// <summary>
    /// Minimum length of provider short title.
    /// </summary>
    public const int MinProviderShortTitleLength = 1;

    /// <summary>
    /// Maximum length of provider short title.
    /// </summary>
    public const int MaxProviderShortTitleLength = 60;

    /// <summary>
    /// Maximum length of position desctiption.
    /// </summary>
    public const int MaxPositionDescriptionLength = 500;

    /// <summary>
    /// Maximum length of additional description.
    /// </summary>
    public const int EnrollmentProcedureDescription = 500;

    /// <summary>
    /// Maximum length of preferential terms of participation.
    /// </summary>
    public const int MaxPreferentialTermsOfParticipationLength = 500;

    /// <summary>
    /// Maximum length of keywords.
    /// </summary>
    public const int MaxKeywordsLength = 200;

    /// <summary>
    /// Maximum length of first name, middle name, and last name for Individual.
    /// </summary>
    public const int MaxIndividualNameLength = 50;

    /// <summary>
    /// Minimum length of first name, middle name, and last name for Individual.
    /// </summary>
    public const int MinIndividualNameLength = 2;

    /// <summary>
    /// Maximum number of employees to upload.
    /// </summary>
    public const int MaxNumberOfEmployeesToUpload = 100;

    /// Length constraint for workshop draft description.
    /// </summary>
    public const int WorkshopDraftDescriptionMaxLength = 500;

    ///<summary>
    /// Length constraint for the description draft items.
    ///</summary>
    public const int WorkshopDraftDescriptionItemsLength = 200;

    ///<summary>
    /// Length constraint for the rejection messages.
    ///</summary>
    public const int WorkshopDraftMaxRejectionMessageLength = 200;

    ///<summary>
    /// Maximum length for disability option descriptions.
    /// </summary>
    public const int DisabilityOptionsLength = 200;

    /// <summary>
    /// Maximum allowed length for a teacher's description.
    /// </summary>
    public const int TeacherDescriptionLength = 300;
    
    /// <summary>
    /// Maximum allowed length for contacts title.
    /// </summary>
    public const int ContactsTitleMaxLength = 60;

    /// <summary>
    /// The maximum length allowed for the competitive event title.
    /// </summary>
    public const int MaxCompetitiveEventTitleLength = 250;

    /// <summary>
    /// The minimum length required for the competitive event title.
    /// </summary>
    public const int MinCompetitiveEventTitleLength = 1;

    /// <summary>
    /// The maximum length allowed for the competitive event short title.
    /// </summary>
    public const int MaxCompetitiveEventShortTitleLength = 100;

    /// <summary>
    /// The minimum length required for the competitive event short title.
    /// </summary>
    public const int MinCompetitiveEventShortTitleLength = 1;

    /// <summary>
    /// The maximum length allowed for the benefits for competitive event.
    /// </summary>
    public const int MaxBenefitsLength = 500;

    /// <summary>
    /// The maximum length allowed for the description.
    /// </summary>
    public const int MaxDescriptionLength = 2000;

    /// <summary>
    /// The maximum length allowed for the venue name.
    /// </summary>
    public const int MaxVenueNameLength = 500;

    /// <summary>
    /// The maximum length allowed for the Judge's description.
    /// </summary>
    public const int MaxJudgeDescriptionLength = 300;
    
    /// <summary>
    /// Sets public images cache control to be 1 hour (GCS default for public data).
    /// </summary>
    public const string PublicImageCacheControl = "public, max-age=3600";
}