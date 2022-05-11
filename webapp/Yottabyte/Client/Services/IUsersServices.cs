using Yottabyte.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Yottabyte.Client.Services
{
    /// <summary>
    /// Interface for the user services
    /// </summary>
    interface IUsersServices
    {
        /// <summary>
        /// Action for checking if a data needs to be updated in the DOM
        /// </summary>
        event Action OnChange;

        /// <summary>
        /// Register handler
        /// </summary>
        /// <param name="user">The user info</param>
        /// <param name="avatarStream">The image as file stream</param>
        /// <param name="filename">Name of the image</param>
        /// <returns>Was register success?</returns>
        Task<String> CreateUser(UserIM user, Stream avatarStream, string filename);
    }
}
