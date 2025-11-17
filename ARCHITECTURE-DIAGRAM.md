# Inventory Archetype - Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         INVENTORY ARCHETYPE PATTERN                          │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                              DOMAIN LAYER                                    │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────┐           │
│  │         InventoryItem (Abstract Base Class)                  │           │
│  ├──────────────────────────────────────────────────────────────┤           │
│  │  + Id: Guid                                                  │           │
│  │  + Name: string                                              │           │
│  │  + Description: string?                                      │           │
│  │  + Category: string?                                         │           │
│  │  + CreatedAt: DateTime                                       │           │
│  │  + CreatedBy: string?                                        │           │
│  │  + LastModifiedAt: DateTime?                                 │           │
│  │  + LastModifiedBy: string?                                   │           │
│  │  + IsDeleted: bool                                           │           │
│  │  + DeletedAt: DateTime?                                      │           │
│  │  + DeletedBy: string?                                        │           │
│  │  + IsStandard: bool                                          │           │
│  │  + MarkAsDeleted(string?)                                    │           │
│  │  + UpdateModificationTracking(string?)                       │           │
│  └──────────────────────────────────────────────────────────────┘           │
│                                    ▲                                         │
│                                    │                                         │
│                     ┌──────────────┴──────────────┐                         │
│                     │                              │                         │
│          ┌──────────────────┐          ┌──────────────────┐                │
│          │   Ingredient      │          │   Future Types   │                │
│          ├──────────────────┤          ├──────────────────┤                │
│          │ + DefaultUnit    │          │   • Meal         │                │
│          │ + CaloriesPer100g│          │   • Recipe       │                │
│          └──────────────────┘          │   • ShoppingList │                │
│                                         │   • Equipment    │                │
│                                         └──────────────────┘                │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                          REPOSITORY LAYER                                    │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────┐           │
│  │     IInventoryRepository<T> where T : InventoryItem          │           │
│  ├──────────────────────────────────────────────────────────────┤           │
│  │  + GetByIdAsync(Guid): Task<T?>                              │           │
│  │  + GetAllAsync(): Task<List<T>>                              │           │
│  │  + GetAllIncludingDeletedAsync(): Task<List<T>>              │           │
│  │  + AddAsync(T): Task                                         │           │
│  │  + UpdateAsync(T): Task                                      │           │
│  │  + DeleteAsync(Guid): Task        // Soft Delete             │           │
│  │  + HardDeleteAsync(Guid): Task    // Permanent Delete        │           │
│  │  + RestoreAsync(Guid): Task       // Restore Deleted Item    │           │
│  └──────────────────────────────────────────────────────────────┘           │
│                                    ▲                                         │
│                                    │ implements                              │
│  ┌──────────────────────────────────────────────────────────────┐           │
│  │     EfInventoryRepository<T> (Abstract Base)                 │           │
│  ├──────────────────────────────────────────────────────────────┤           │
│  │  # _db: ApplicationDbContext                                 │           │
│  │  # DbSet: DbSet<T> (abstract)                                │           │
│  │  + Implements all IInventoryRepository<T> methods            │           │
│  │  + Automatic soft delete filtering via EF Query Filters      │           │
│  │  + Automatic temporal tracking on updates                    │           │
│  └──────────────────────────────────────────────────────────────┘           │
│                                    ▲                                         │
│                                    │ extends                                 │
│                     ┌──────────────┴──────────────┐                         │
│                     │                              │                         │
│          ┌────────────────────┐        ┌────────────────────┐              │
│          │ IIngredientRepo    │        │ IRecipeRepository  │              │
│          │ : IInventoryRepo   │        │ : IInventoryRepo   │              │
│          └────────────────────┘        └────────────────────┘              │
│                     ▲                              ▲                         │
│          ┌────────────────────┐        ┌────────────────────┐              │
│          │ EfIngredientRepo   │        │ EfRecipeRepository │              │
│          │ : EfInventoryRepo  │        │ : EfInventoryRepo  │              │
│          └────────────────────┘        └────────────────────┘              │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                         BENEFITS OF THE PATTERN                              │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ✅ REUSABILITY                                                              │
│     • Single implementation serves multiple entity types                    │
│     • Add new inventory types in ~10 lines of code                          │
│                                                                              │
│  ✅ CONSISTENCY                                                              │
│     • All inventory items behave identically                                │
│     • Uniform soft delete, restore, and audit behavior                      │
│                                                                              │
│  ✅ MAINTAINABILITY                                                          │
│     • Changes to inventory logic applied everywhere                         │
│     • Single source of truth for inventory operations                       │
│                                                                              │
│  ✅ AUDIT TRAIL                                                              │
│     • Complete temporal tracking (Created, Modified, Deleted)               │
│     • Who performed each action (when user context available)               │
│                                                                              │
│  ✅ DATA PRESERVATION                                                        │
│     • Soft delete prevents accidental data loss                             │
│     • Ability to restore deleted items                                      │
│     • Historical data remains for analysis                                  │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Example: Adding a New Inventory Type

```csharp
// Step 1: Create Model
public class Recipe : InventoryItem
{
    public int PrepTimeMinutes { get; set; }
    public string? Instructions { get; set; }
}

// Step 2: Create Repository Interface
public interface IRecipeRepository : IInventoryRepository<Recipe>
{
    // Add recipe-specific methods here if needed
}

// Step 3: Create Repository Implementation
public class EfRecipeRepository : EfInventoryRepository<Recipe>, IRecipeRepository
{
    public EfRecipeRepository(ApplicationDbContext db) : base(db) { }
    
    protected override DbSet<Recipe> DbSet => _db.Recipes;
    
    // Add recipe-specific implementations here if needed
}

// Step 4: Configure in DbContext
modelBuilder.Entity<Recipe>(b =>
{
    b.HasKey(x => x.Id);
    b.Property(x => x.Name).IsRequired();
    b.HasQueryFilter(x => !x.IsDeleted);  // Enable soft delete
});

// Step 5: Register in DI Container (Program.cs)
builder.Services.AddScoped<IRecipeRepository, EfRecipeRepository>();
```

That's it! Recipe now has:
- Soft delete
- Restore capability
- Full audit trail
- Temporal tracking
- All standard CRUD operations
