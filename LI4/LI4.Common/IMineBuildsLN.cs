using LI4.Common.Dados;
using LI4.Dados;

namespace LI4.Common;

/// <summary>
/// Defines the methods that need to be implemented for handling the business logic
/// related to Minebuilds. This interface serves as a contract for the services that
/// manage the core functionality of the Minebuilds system, such as managing
/// constructions, construction stages, users, resources and stock.
/// </summary>
public interface IMineBuildsLN {

    #region//---- USER METHODS ----//
    /// <summary>
    /// Retrieves all users from the system asynchronously, by accessing the database.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is an <see cref="IEnumerable{User}"/> containing all the users in the system.
    /// </returns>
    public Task<IEnumerable<User>> getAllUsersAsync();

    /// <summary>
    /// Authenticates a user by verifying the provided email and password.
    /// This method checks if a user with the given email and password exists in the system.
    /// If successful, the user details are returned; otherwise, an exception is thrown.
    /// </summary>
    /// <param name="email">The email address of the user to authenticateAsync.</param>
    /// <param name="password">The password associated with the user's account.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is a <see cref="User"/>
    /// object containing the authenticated user's details, if the authentication is successful.
    /// </returns>
    /// <exception cref="LI4.Common.Exceptions.UserExceptions.UserNotFoundException">
    /// Thrown when no user is found with the provided email and password.
    /// </exception>
    public Task<User> authenticateAsync(string email, string password);

    /// <summary>
    /// Updates the details of an existing user in the system.
    /// This method allows you to update the user's email, username, and password.
    /// </summary>
    /// <param name="id">The unique identifier of the user to be updated.</param>
    /// <param name="email">The new email address of the user.</param>
    /// <param name="username">The new username of the user.</param>
    /// <param name="password">The new password of the user.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the updated user object.</returns>
    /// <exception cref="LI4.Common.Exceptions.UserExceptions.UserNotFoundException">
    /// Thrown when the user with the specified id does not exist in the system.
    /// </exception>
    /// <exception cref="LI4.Common.Exceptions.UserExceptions.UserAlreadyExistsException">
    /// Thrown when the new email or username already exists in the system, causing a conflict.
    /// </exception>
    public Task<User> updateUserAsync(int id, string email, string username,string password);

    /// <summary>
    /// Retrieves the user information based on the provided email.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the user object if found.</returns>
    public Task<User> getUserByEmailAsync(string email);

    /// <summary>
    /// Registers a new user with the provided email, username, and password.
    /// </summary>
    /// <param name="email">The email address of the user to register. Must be unique in the system.</param>
    /// <param name="username">The username of the user to register. Must be unique in the system.</param>
    /// <param name="password">The password for the new user account.</param>
    /// <returns>A task representing the asynchronous operation. Returns <c>true</c> if the registration is successful, <c>false</c> if the registration 
    /// fails (e.g., if the email or username is already taken).</returns>
    public Task<bool> registerUserAsync(string email, string username, string password);
    #endregion

    #region//---- STOCK METHODS----//
    /// <summary>
    /// Retrieves all blocks associated with the user.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, with a result containing a collection of blocks.</returns>
    public Task<IEnumerable<Block>> getAllBlocksAsync();

    /// <summary>
    /// Retrieves the content of a specific order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing a dictionary of block names and ther quantities for the specified order.</returns>
    public Task<Dictionary<string, int>> getOrderContentAsync(int id);

    /// <summary>
    /// Retrieves the list of orders associated with a user.
    /// </summary>
    /// <param name="id"> The ID of the user.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing a list of orders for the specified user.</returns>
    public Task<List<Order>> getUserOrdersAsync(int id);

    /// <summary>
    /// Retrieves the current stock of blocks associated with a user.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing a dictionary of block names and their respective quantities in the stock</returns>
    public Task<Dictionary<string, int>> getUserStockAsync(int id);

    /// <summary>
    /// Creates a new order for the user with the specified blocks and quantities.
    /// </summary>
    /// <param name="id">The ID of the user placing the order.</param>
    /// <param name="blocks">A dictionary containing the block ID and the quantity of each block for the order.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the ID of the newly created order.</returns>
    public Task<int> createOrderAsync(int id, Dictionary<int, int> blocks);

    /// <summary>
    /// Retrieves the properties of a specific block type by its ID.
    /// </summary>
    /// <param name="blockPropertiesID">The ID of the block properties to retrieve.</param>
    /// <returns>A <see cref="BlockProperties"/> object containing the details of the specified block.</returns>
    public BlockProperties getBlockProperties(int blockPropertiesID);

    Dictionary<int, BlockProperties> getAllBlockProperties();
    #endregion

    #region//---- CONSTRUCTION METHODS ----//
    /// <summary>
    /// Retrieves all construction instances.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, with a result containing a collection of all construction instances.</returns>
    public Task<IEnumerable<Construction>> getAllConstructionInstancesAsync();

    /// <summary>
    /// Checks if the user has the necessary stock of blocks to build a specific construction.
    /// </summary>
    /// <param name="constructionPropertiesID">The ID of the construction properties.</param>
    /// <param name="userID">The ID of the user.</param>
    /// <returns>A task representing the asynchronous operation, with a result indicating whether the user has enough blocks (true if they have enough stock, false otherwise).</returns>
    public Task<bool> hasStockAsync(int userID, int constructionPropertiesID);

    /// <summary>
    /// Adds a construction to the user's construction queue.
    /// </summary>
    /// <param name="userID">The ID of the user placing the construction order.</param>
    /// <param name="constructionPropertiesID">The ID of the construction property to add to the queue.</param>
    /// <returns>A task representing the asynchronous operation, with a result indicating whether the construction was successfully added to the queue.</returns>
    public Task<bool> addConstructionToQueueAsync(int userID, int constructionPropertiesID);

    /// <summary>
    /// Gets the current assembly line stage number of the construction.
    /// </summary>
    /// <param name="idConstruction">The ID of the construction to get the current stage position.</param>
    /// <param name="idUser">The ID of the user that owns the construction.</param>
    /// <param name="idConstructionProperties">The ID of the construction property to filter the assembly line.</param>
    /// <returns>A integer with the current stage number.</returns>
    public int getConstructionCurrentStage(int idConstruction, int idUser, int idConstructionProperties);

    /// <summary>
    /// Calculates the missing blocks needed for a specific construction by a user.
    /// </summary>
    /// <param name="userdID">The ID of the user.</param>
    /// <param name="constructionPropertiesID">The ID of the construction property.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing a dictionary of missing blocks (block names and quantities).</returns>
    public Task<Dictionary<string, int>> calculateMissingBlocksAsync(int userdID, int constructionPropertiesID);

    /// <summary>
    /// Retrieves the constructions that are currently on the waiting queue for the specified user.
    /// </summary>
    /// <param name="userID">The ID of the user whose waiting constructions are to be retrieved.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing a dictionary of constructions in the queue (keyed by construction property id and the corresponding quantities).</returns>
    public Task<Dictionary<int, int>> getAwaitingConstructionsAsync(int userID);

    /// <summary>
    /// Retrieves the list of completed constructions for the user.
    /// </summary>
    /// <param name="userID">The ID of the user.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing a dictionary of completed constructions (keyed by constructio property id and the corresponding quantities).</returns>
    public Task<Dictionary<int, int>> getCompletedConstructionsAsync(int userID);

    /// <summary>
    /// Retrieves the constructions that are currently being built by the specified user.
    /// </summary>
    /// <param name="userID">The ID of the user whose building constructions are to be retrieved.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing a dictionary of constructions in the assembly line (keyed by construction property id and the corresponding quantities).</returns>
    public Task<Dictionary<int, int>> getBuildingConstructionsAsync(int userID);

    /// <summary>
    /// Views the details of a completed construction by the user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="constructionId">The ID of the completed construction.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing a dictionary of completed constructions (keyed by construction names and the corresponding quantities).</returns>
    public Task<Dictionary<string, int>> viewCompletedConstructionAsync(int userId, int constructionId);

    /// <summary>
    /// Removes a construction instance from the user's construction queue.
    /// </summary>
    /// <param name="idUser">The ID of the user.</param>
    /// <param name="idConstructionProperties">The ID of the construction to remove from the queue.</param>
    /// <returns>A task representing the asynchronous operation, with a result indicating whether the construction was successfully removed.</returns>
    public Task<bool> removeConstructionAsync(int idUser, int idConstructionProperties);

    /// <summary>
    /// Retrieves the catalog of available construction.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, with a result containing a dictionary of construction property IDs and their respective id.</returns>
    public List<int> getCatalog();

    /// <summary>
    /// Retrieves the properties of a specific construction type by its ID.
    /// </summary>
    /// <param name="constructionPropertiesID">The ID of the construction properties to retrieve.</param>
    /// <returns>A <see cref="ConstructionProperties"/> object containing the details of the specified construction property.</returns>
    public ConstructionProperties getConstructionProperties(int constructionPropertiesID);

    /// <summary>
    /// Retrieves the required blocks for a specific construction and block property.
    /// </summary>
    /// <param name="constructionPropertiesID">The ID of the construction properties.</param>
    /// <param name="blockPropertiesID">The ID of the block property needed for the construction.</param>
    /// <returns>A <see cref="BlocksToConstruction"/> object containing the block properties and their quantities required for the construction.</returns>
    public BlocksToConstruction getBlocksToConstruction(int constructionPropertiesID, int blockPropertiesID);

    public Task<Dictionary<string, int>> getAllBlocksConstructionAsync(int constructionPropertiesID);

    public Dictionary<string, int> getBlocksAtStageConstruction(int constructionPropertiesID, int stage);

    public Task<List<int>> getBuildingIdsConstructionsAsync(int idUser, int idConstructionProperties);

    public int getEstimatedTime(int idUser, int idConstructionProperties, int stage);

    public Task<bool> addConstructionToQueueBatchAsync(Dictionary<int, int> blocksNeeded, int userID, int constructionPropertiesID, int quantity);

    public Task<int> getConstructionPropertyIdAsync(int idConstruction);

    #endregion
}
