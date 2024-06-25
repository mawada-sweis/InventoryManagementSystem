# InventoryManagementSystem
The Inventory Management System project aims to simplify inventory management for administrators. It provides functionalities for managing items, categories, and user authentication. This user guide will walk you through the setup, configuration, and usage of the system.

## Prerequisites
Before using the System, ensure you have the following prerequisites installed:
- PostgreSQL database
- NUnit (for running tests)
## Installation
To install the Inventory Management System:
- Download the project from the GitHub repository.
- Install NUnit using your preferred package manager.
## Configuration
To configure the Inventory Management System, store your PostgreSQL database password in an environment variable named "PG_PASSWORD".
## Usage Instructions
### Authentication Service
- Registering a new user

Use the `Register` method of the authentication service to register a new user
- Logging in

Use the `Login` method of the authentication service to log in with an email and password.
- Resetting passwords

Use the `resetPassword` method of the authentication service to reset a user's password.
- Getting user information

Use the `GetUserInfo` method of the authentication service to retrieve user information.
### Categories Service
- Getting Categories:

Use the `GetCategories` method of the categories service to retrieve a list of categories.
- Adding a New Category:

Use the `AddCategory` method of the categories service to add a new category.
- Deleting a Category:

Use the `DeleteCategory` method of the categories service to delete a category.
- Updating a Category:

Use the `UpdateCategory` method of the categories service to update a category.
### Item Service
- Getting Items:

Use the `GetItems` method of the item service to retrieve a list of items.
- Adding a New Item:

Use the `AddItem` method of the item service to add a new item.
- Updating an Item:

Use the `UpdateItem` method of the item service to update an item.
- Deleting an Item:

Use the `DeleteItem` method of the item service to delete an item.
- Updating Item Quantity:

Use the `UpdateQuantity` method of the item service to update the quantity of an item.
- Updating Sold Items:

Use the `UpdateSoldItem` method of the item service to update the sold quantity of an item.
- Searching for Items:

Use the `GetItemByName` method of the item service to search for an item by name.
- Filtering Items Based on Criteria:

Use the `GetFilterItems` method of the item service to filter items based on specific criteria.
