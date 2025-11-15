# MealForToday - Application Specification

## Project Overview

**MealForToday** is a meal planning application designed to help users plan their meals for varying time periods (daily, weekly, or monthly). The application enables users to create and manage preferred meals, generate meal schedules, and add custom meals based on individual ingredients and quantities.

---

## Core Features

### 1. Preferred Meals Management
**Description:** Users can create, manage, and maintain a library of their favorite meals with associated ingredients.

#### Key Functionality:
- **Create Meal**: Add a new meal to the preferred meals list
  - Input: Meal name, description, cuisine type (optional)
  - Store associated ingredients with quantities
  
- **View Meals**: Display all preferred meals in a list/grid format
  - Show meal details (name, ingredients, nutritional info if available)
  - Quick preview of meal composition
  
- **Edit Meal**: Modify existing meal details
  - Update meal name, description, ingredients
  - Adjust ingredient quantities
  
- **Delete Meal**: Remove meals from the preferred list
  - Confirmation dialog before deletion
  - Archive option (optional future enhancement)

- **Search & Filter**: Quick access to meals
  - Search by meal name
  - Filter by cuisine type, ingredients, dietary preferences
  - Favorite/starred meals feature

#### Data Model:
```
Meal
├── Id (GUID)
├── Name (string)
├── Description (string)
├── CreatedDate (DateTime)
├── LastModifiedDate (DateTime)
└── Ingredients (List<MealIngredient>)
    ├── IngredientId (GUID)
    ├── IngredientName (string)
    ├── Quantity (decimal)
    ├── Unit (enum: g, ml, tbsp, tsp, cup, piece, etc.)
    └── Notes (string, optional)
```

---

### 2. Meal Schedule Generation
**Description:** Automatically generate meal schedules for specified time periods based on user's preferred meals.

#### Key Functionality:
- **Generate Schedule**: Create meal plans for custom periods
  - Input: Time period (daily, weekly, monthly), number of meals per day
  - Algorithm: Randomly select from preferred meals, with optional constraints.
  
- **View Schedule**: Display generated meal plan
  - Calendar or list view
  - Show meal for each time slot
  - Display ingredients needed for the entire schedule
  
- **Customize Schedule**: Manually adjust generated schedules
  - Move meals between days
  - Replace specific meals with others
  - Mark meals as "locked" to prevent changes during regeneration
  
- **Shopping List Generation**: Aggregate ingredients from entire schedule
  - Combine ingredient quantities across meals
  - Group by ingredient type
  - Export as printable list or shareable format

- **Schedule Constraints** (optional enhancements):
  - Exclude certain meals from appearing too frequently
  - Balance cuisine types throughout the period
  - Dietary preference filters (vegetarian, vegan, gluten-free, etc.)
  - Avoid repeated meals on consecutive days

#### Data Model:
```
MealSchedule
├── Id (GUID)
├── UserId (GUID)
├── StartDate (DateTime)
├── EndDate (DateTime)
├── CreatedDate (DateTime)
└── ScheduleEntries (List<ScheduleEntry>)
    ├── EntryId (GUID)
    ├── MealId (GUID)
    ├── Date (DateTime)
    ├── MealType (enum: Breakfast, Lunch, Dinner, Snack)
    └── Notes (string, optional)
```

---

### 3. Custom Meal Creation from Raw Ingredients
**Description:** Users can create meals by directly specifying ingredients and their quantities without using the predefined meal library.

#### Key Functionality:
- **Add Meal from Ingredients**: Create meal using raw ingredient input
  - Input: Meal name, list of ingredients with quantities
  - Quick entry form or table-based input
  
- **Ingredient Input Options**:
  - Manual entry: Name + quantity + unit
  - Autocomplete: Suggest common ingredients from database
  - Batch input: Paste ingredients in text format
  
- **Ingredient Database**: Maintain a common ingredient library
  - Store standard ingredient names
  - Map variations (e.g., "cheddar cheese" → "cheese")
  - Support custom ingredients not in library
  
- **Save Custom Meal**: Option to add created meal to preferred list
  - Add to meal library for future use
  - Save as one-time meal entry in schedule
  
- **Recent Ingredients**: Quick access to frequently used ingredients
  - Auto-suggestions based on usage history

#### Data Model:
```
Ingredient
├── Id (GUID)
├── Name (string)
├── Category (enum: Protein, Vegetable, Fruit, Dairy, Grain, Spice, Other)
├── DefaultUnit (string)
├── CaloriesPer100g (decimal, optional)
└── IsStandard (bool)

CustomMealEntry
├── Id (GUID)
├── UserId (GUID)
├── Name (string)
├── Ingredients (List<Ingredient> with quantities)
├── CreatedDate (DateTime)
└── IsSavedToLibrary (bool)
```

---

## User Interface Components

### Pages/Views:
1. **Dashboard**: Overview of meal planning status
   - Recent schedules
   - Quick actions (create meal, generate schedule, add custom meal)
   - Statistics (total meals, schedules created)

2. **Meals Management Page**:
   - List/grid of preferred meals
   - Add/Edit/Delete meal forms
   - Search and filter controls

3. **Schedule Generation Page**:
   - Form to select period type and duration
   - Schedule constraints/preferences
   - Generated schedule display (calendar or list view)

4. **Schedule Detail Page**:
   - View/edit generated schedule
   - Shopping list view
   - Option to regenerate or modify

5. **Custom Meal Entry Page**:
   - Form for adding custom meals
   - Ingredient input table/form
   - Option to save to library

6. **Shopping List Page**:
   - View aggregated shopping list
   - Check off items
   - Export/print functionality

---

## Technical Architecture

### Backend (MealForToday.Application)
- **Services**:
  - `MealService`: CRUD operations for meals
  - `ScheduleService`: Schedule generation and management
  - `IngredientService`: Manage ingredient database
  
- **Repositories**:
  - `IMealRepository`
  - `IScheduleRepository`
  - `IIngredientRepository`
  
- **Algorithms**:
  - Schedule generation algorithm with constraint satisfaction

### Frontend (MealForToday.UI)
- **Blazor Components** for each feature area
- **State Management**: Application state for user's meals and schedules
- **Forms**: Input validation and user feedback
- **Responsive Design**: Support desktop and mobile views

### Database (Entity Framework Core)
- **Models**: As defined in data models section
- **Migrations**: Database schema versioning
- **Relationships**: Foreign keys between Meal, Schedule, and Ingredient entities

---

## Implementation Phases

### Phase 1: Foundation (MVP)
1. Create database models and migrations
2. Implement Meal CRUD operations
3. Basic UI for meal management
4. User authentication setup

### Phase 2: Schedule Generation
1. Implement schedule generation algorithm
2. Create schedule visualization (calendar/list)
3. Shopping list generation
4. Manual schedule customization

### Phase 3: Custom Meals & Enhancement
1. Ingredient database setup
2. Custom meal entry form
3. Ingredient autocomplete
4. Recent ingredients feature

### Phase 4: Advanced Features (Future)
1. Nutritional information tracking
2. Cost estimation for shopping lists
3. Dietary preference filters
4. Export/sharing functionality
5. Mobile app consideration
6. Integration with recipe APIs

---

## Non-Functional Requirements

- **Performance**: Schedule generation should complete within 2 seconds
- **Scalability**: Support 10,000+ meals and 1,000+ schedules per user
- **Reliability**: 99.9% uptime with data backup
- **Security**: User data encryption, secure authentication
- **Usability**: Intuitive UI with helpful tooltips and error messages
- **Accessibility**: WCAG 2.1 AA compliance

---

## Success Criteria

1. Users can successfully create and manage preferred meals
2. Meal schedules can be generated within acceptable timeframe
3. Shopping lists accurately aggregate ingredients
4. Custom meal creation works smoothly
5. System is intuitive and requires minimal training
6. User satisfaction score > 4/5 stars

---

## Future Enhancements

- Nutritional tracking and dietary recommendations
- Integration with recipe APIs (Spoonacular, Edamam, etc.)
- Meal ratings and reviews
- Social sharing of meal plans
- AI-powered meal recommendations based on preferences
- Barcode scanning for shopping list tracking
- Price comparison and deals integration
- Cooking time estimation
- Meal prep guides and cooking instructions
- Mobile app version

---

## Dependencies & Tools

- **.NET 9** / **C#**
- **Blazor** (UI Framework)
- **Entity Framework Core** (ORM)
- **SQL Server** or **SQLite** (Database)
- **Bootstrap** (CSS Framework)
- **AutoMapper** (Object mapping, optional)

---

## Assumptions & Constraints

- Users will have modern web browsers (Chrome, Firefox, Safari, Edge)
- Internet connection required for application access
- Initial user base is English-speaking (future i18n support)
- Meals are created per-user (multi-tenant consideration for future)
- No real-time collaboration initially

