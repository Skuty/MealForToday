# Inventory Archetype Pattern Implementation

## Overview

This implementation follows the **Inventory Archetype** pattern, a domain modeling pattern commonly described in Martin Fowler's "Analysis Patterns". The pattern provides a reusable structure for managing items that need to be tracked in an inventory-style system with full audit capabilities.

## Key Components

### 1. InventoryItem Base Class (`Models/InventoryItem.cs`)

The foundation of the pattern, providing common properties and behaviors for all inventory items:

**Core Properties:**
- `Id` - Unique identifier
- `Name` - Item name
- `Description` - Optional detailed description
- `Category` - Grouping/classification

**Temporal Tracking:**
- `CreatedAt` / `CreatedBy` - Creation audit trail
- `LastModifiedAt` / `LastModifiedBy` - Modification tracking
- `DeletedAt` / `DeletedBy` - Deletion tracking

**Soft Delete Support:**
- `IsDeleted` - Marks items as deleted without removing data
- `MarkAsDeleted()` - Method to properly soft delete items
- `UpdateModificationTracking()` - Method to track changes

**Metadata:**
- `IsStandard` - Indicates system-provided vs user-created items

### 2. Generic Repository Interface (`Repositories/IInventoryRepository.cs`)

Defines standard operations for inventory items:

```csharp
public interface IInventoryRepository<T> where T : InventoryItem
{
    Task<T?> GetByIdAsync(Guid id);
    Task<List<T>> GetAllAsync();
    Task<List<T>> GetAllIncludingDeletedAsync();
    Task AddAsync(T item);
    Task UpdateAsync(T item);
    Task DeleteAsync(Guid id);          // Soft delete
    Task HardDeleteAsync(Guid id);      // Permanent delete
    Task RestoreAsync(Guid id);         // Restore soft-deleted items
}
```

### 3. Generic Repository Implementation (`Repositories/EfInventoryRepository.cs`)

Abstract base class providing concrete implementation of inventory operations using Entity Framework Core:

**Features:**
- Automatic query filtering (excludes soft-deleted items)
- Temporal tracking on updates
- Soft delete with audit trail
- Restore capability for deleted items
- Hard delete for permanent removal

### 4. Specific Implementation (Ingredient)

**Model** (`Models/Ingredient.cs`):
```csharp
public class Ingredient : InventoryItem
{
    public string? DefaultUnit { get; set; }
    public decimal? CaloriesPer100g { get; set; }
}
```

**Repository Interface** (`Repositories/IIngredientRepository.cs`):
```csharp
public interface IIngredientRepository : IInventoryRepository<Ingredient>
{
    // Additional ingredient-specific methods can be added here
}
```

**Repository Implementation** (`Repositories/EfIngredientRepository.cs`):
```csharp
public class EfIngredientRepository : EfInventoryRepository<Ingredient>, IIngredientRepository
{
    public EfIngredientRepository(ApplicationDbContext db) : base(db) { }
    protected override DbSet<Ingredient> DbSet => _db.Ingredients;
}
```

## Benefits

### 1. Reusability
The pattern can be applied to any entity needing inventory-style management:
- Ingredients (current implementation)
- Meals (future)
- Recipes (future)
- Shopping lists (future)
- Any other tracked items

### 2. Consistency
All inventory items behave the same way:
- Same CRUD operations
- Same soft delete behavior
- Same audit tracking
- Same temporal data

### 3. Maintainability
- Changes to inventory behavior apply to all items
- Single source of truth for inventory logic
- Easy to add new inventory types

### 4. Audit Trail
Complete tracking of:
- When items were created, modified, and deleted
- Who performed each action (when user context is available)
- Ability to restore accidentally deleted items

### 5. Data Preservation
Soft delete ensures:
- No data loss from accidental deletion
- Historical records remain intact
- Ability to analyze deleted items
- Compliance with data retention policies

## Usage Examples

### Using the Generic Repository Directly

The generic repository can be used directly without creating concrete repository types:

```csharp
// In Program.cs (DI registration)
builder.Services.AddScoped<IInventoryRepository<Ingredient>>(
    sp => new EfInventoryRepository<Ingredient>(
        sp.GetRequiredService<ApplicationDbContext>(),
        db => db.Ingredients
    )
);

// In Service
public class IngredientService : IIngredientService
{
    private readonly IInventoryRepository<Ingredient> _repo;
    
    public IngredientService(IInventoryRepository<Ingredient> repo)
    {
        _repo = repo;
    }
    
    // Use repository methods directly
}
```

### Creating a New Inventory Type

To add a new inventory type (e.g., Recipe), simply:

```csharp
// 1. Create the model
public class Recipe : InventoryItem
{
    public int PrepTimeMinutes { get; set; }
    public string? Instructions { get; set; }
}

// 2. Register in DI container
builder.Services.AddScoped<IInventoryRepository<Recipe>>(
    sp => new EfInventoryRepository<Recipe>(
        sp.GetRequiredService<ApplicationDbContext>(),
        db => db.Recipes
    )
);

// 3. Configure in DbContext
modelBuilder.Entity<Recipe>(b =>
{
    b.HasKey(x => x.Id);
    b.Property(x => x.Name).IsRequired();
    b.HasQueryFilter(x => !x.IsDeleted);  // Enable soft delete
});
```

That's it! No need to create concrete repository interfaces or implementations.

### Using the Repository

```csharp
// Create
var ingredient = new Ingredient 
{ 
    Name = "Tomato",
    Description = "Fresh red tomato",
    DefaultUnit = "piece",
    Category = "Vegetable"
};
await repository.AddAsync(ingredient);

// Update (automatically tracks modification time)
ingredient.Name = "Cherry Tomato";
await repository.UpdateAsync(ingredient);

// Soft Delete (preserves data)
await repository.DeleteAsync(ingredient.Id);

// Restore
await repository.RestoreAsync(ingredient.Id);

// Hard Delete (permanent)
await repository.HardDeleteAsync(ingredient.Id);

// Query
var active = await repository.GetAllAsync();           // Only active items
var all = await repository.GetAllIncludingDeletedAsync(); // All items
```

## Testing

The pattern includes comprehensive tests (`IngredientServiceTests.cs`) covering:

1. **Basic CRUD**: Create, read, update, delete
2. **Soft Delete**: Verifies items are hidden but preserved
3. **Data Preservation**: Ensures deleted data remains intact
4. **Restore**: Verifies deleted items can be restored
5. **Temporal Tracking**: Verifies creation, modification, and deletion timestamps
6. **Query Filtering**: Ensures deleted items are automatically excluded

## EF Core Configuration

The `ApplicationDbContext` must include a query filter for each inventory entity:

```csharp
modelBuilder.Entity<Ingredient>(b =>
{
    b.HasKey(x => x.Id);
    b.Property(x => x.Name).IsRequired();
    b.HasQueryFilter(x => !x.IsDeleted);  // Automatic soft delete filtering
});
```

This ensures that soft-deleted items are automatically excluded from queries unless explicitly requested with `IgnoreQueryFilters()`.

## Future Enhancements

The inventory archetype can be extended with:

1. **Versioning**: Track version history of items
2. **Quantity Management**: Track stock levels and reservations
3. **Location Tracking**: Track where items are stored
4. **Transaction History**: Log all inventory movements
5. **Expiration Dates**: Track when items expire
6. **Batch/Lot Numbers**: Track items by production batch
7. **Multi-tenancy**: Support for multiple organizations

## References

- Martin Fowler's "Analysis Patterns" - Inventory Archetype
- Domain-Driven Design principles
- Entity Framework Core documentation on query filters
- Audit trail best practices
