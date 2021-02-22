using System;

namespace Sogetrel.Sinapse.Framework.Exceptions
{
    [Serializable]
    /// <summary>
    /// WIP : Exception spécifique aux erreurs métiers définies dans les classes Specifications. 
    /// </summary>
    public class SpecificationException : Exception
    {
        public string ErrorCode { get; set; }
        public string Description { get; set; }
        public string Cause { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="errorCode">Code d'erreur métier specifié dans les specifications</param>
        /// <param name="description">Description d'erreur métier specifiée dans les specifications</param>
        /// <param name="cause">Cause de l'erreur métier specifiée dans les specifications</param>
        public SpecificationException(string errorCode, string description, string cause)
        {
            ErrorCode = errorCode;
            Description = description;
            Cause = cause;
        }
    }
}
