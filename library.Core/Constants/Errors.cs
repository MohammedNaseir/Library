namespace library.Core.Constants
{
    public static class Errors
    {
        public const string MaxLength = "lenth cannot be more than {1} characters";
        public const string MaxMinLength = "The {0} must be at least {2} and at max {1} characters long.";
        public const string Duplicated = "{0} with the same name is already exists";
        public const string NotAllowedExtension = "Only .png , .jpge files are allowed";
        public const string MaxSize = "File Cannot be more that 2 MG";
        public const string DuplicatedBook = "Book with the same title wirh the same author";
        public const string NotAllowFutureDates = "Date Cannot be in the future ";
        public const string invalidEditionNumber = "{0} should be between {1} and {2}";
        public const string ConfirmPassNotMatche = "The password and confirmation password do not match.";
        public const string WeakPassword = "Passwords contain an uppercase character, lowercase character, a digit, and a non-alphanumeric character. Passwords must be at least 8 characters long";
        public const string InvalidUsername = "Username can only contain letters or digits.";
        public const string OnlyEnglishLetters = "Only English letters are allowed.";
        public const string OnlyArabicLetters = "Only Arabic letters are allowed.";
        public const string OnlyNumbersAndLetters = "Only Arabic/English letters or digits are allowed.";
        public const string DenySpecialCharacters = "Special characters are not allowed.";
        public const string RequiredField = "Required Field";
		public const string InvalidMobileNumber = "Invalid mobile number.";
		public const string InvalidNationalId = "Invalid national ID.";
		public const string EmptyImage = "Please select an image.";
		public const string InvalidSerialNumber = "Invalid Serial Number.";
		public const string NotAvailableForRental = "This Copy / Rental Not Available For Rental.";
		public const string BlackListedSubscriber = "This Subscriber is BlackList";
		public const string InactiveSubscriber = "This Subscriber is Inactive";
        public const string MaxCopiesReached = "This subscriber has reached the max number for rentals.";
        public const string CopyIsInRental = "This copy is already rentaled.";
        public const string RentalNotAllowedForBlacklisted = "Rental cannot be extended for blacklisted subscribers.";
        public const string RentalNotAllowedForInactive = "Rental cannot be extended for this subscriber before renwal.";
        public const string ExtendNotAllowed = "Rental cannot be extended.";
        public const string PenaltyShouldBePaid = "Penalty should be paid.";

    }
}
