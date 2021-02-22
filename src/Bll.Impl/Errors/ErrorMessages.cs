namespace Assistance.Operational.Bll.Impl.Errors
{
    public static class ErrorMessages
    {
        public static readonly string userAlreadyExistErrorMessage = "A similar user has already been created using this Email. Maybe your company already created an account for you. If so, please click on the link that you received in your invitation email.";
        public static readonly string companyAlreadyExistErrorMessage = "A similar company has already been created using this SIRET number. Maybe your company already created an account for you. If so, please click on the link that you received in your invitation email.";
        public static readonly string siretCannotBeNull = "The SIRET cannot be null.";
        public static readonly string emailCannotBeNull = "The email cannot be null";
        public static readonly string userCannotBeNull = "The user cannot be null";
        public static readonly string wrongUserRole = "wrong user role";
        public static readonly string commercialCannotBeNull = "Une mission ne peut pas être tripartite et sans commercial";
        public static readonly string atLeastTwoUsersNeeded = "Two users are needed to create a mission. Three are needed if there is a commercial entity";
        public static readonly string errorOccuredWhileGettingUsers = "An error occured while getting the users";
        public static readonly string mandatoryParameterMissing = "Mandatory parameter missing";
        public static readonly string couldNotProcess = "We understood your request, but we could not process the requested object. Please check the documentation.";
        public static readonly string notFound = "We understood your request, but we could not find the requested object.";

        public static readonly string registerUserBadFormat = "One of the inputs is not well formated. Inputs: email, firstname, lastname, siret";
        public static readonly string companyBadFormat = "One of the inputs is not well formated. Inputs: ";
        public static readonly string userNotPermittedToSign = "This user is not permitted to sign this activity report.";
        public static readonly string youCannotSignActivityAtThisTime = "You cannot sign this activity report at this time. You have to wait other users to sign first.";
        public static readonly string activityDoesntExist = "The activity report doesn't exist";
        public static readonly string onlyCustomerAndCommercialCanRequireActivityModification = "Seuls les clients et les commerciaux peuvent demander une modification du CRA.";
        public static readonly string consultantMustHaveSignedActivity = "Le consultant doit avoir signé son rapport d'activité avant la demande de modification.";
        public static readonly string missionDoesntExist = "The mission doesn't exist";
        public static readonly string userDoesntExist = "The user doesn't exist";
        public static readonly string errorOccuredWhileSigningActivity = "An error occured while signing the activity report.";
        public static readonly string dateMismatchError = "The end date cannot be superior to the end date";
        public static readonly string userCannotSignYet = "The user cannot sign the activity yet.";
        public static readonly string createOrgaNoNameError = "The name property is mandatory.";
        public static readonly string organizationDoesntExist = "The organization doesn't exist";
        public static readonly string inviteRevoked = "Cette invitation est révoquée et ne peut plus être utilisée.";
        public static readonly string inviteExpired = "Cette invitation est expirée et ne peut plus être utilisée.";
        public static readonly string tokenError = "Token error";
        public static readonly string tokenUnsupported = "Token unsupported";
        public static readonly string tokenIdentityCompromised = "The identity has been compromised";
        public static readonly string invitationCodeRequired = "Invitation code is required";
        public static readonly string invitationDoesntExist = "Invitation does not exits";
        public static readonly string badUserRole = "Cet utilisateur n'a pas les droits requis pour effectuer cette action.";
        public static readonly string badActivityStatus = "Le status du CRA doit être 'Modifications Requises'.";
        public static readonly string activityModificationsNotExist = "Aucune proposition de modification n'existe pour ce CRA.";
        public static readonly string authorDoesntExist = "L'auteur de cette modification n'existe plus.";
        public static readonly string emailNeeded = "We need the emails of those who are not yet invited";
    }
}
