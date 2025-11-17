using System;

namespace MealForToday.Application.Models
{
    /// <summary>
    /// Defines standard units of measurement with their base conversion factors.
    /// Used to calculate ingredient quantities as multiples of default amounts.
    /// </summary>
    public class UnitDefinition
    {
        /// <summary>
        /// Unique identifier for the unit definition
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Unit code (e.g., "kg", "g", "l", "ml")
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Full name of the unit (e.g., "Kilogram", "Gram", "Litre", "Milliliter")
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Base amount multiplier. This represents how many of this unit make up the base measure.
        /// Example: 
        /// - Kilogram: 1 (base unit for weight)
        /// - Gram: 1000 (1000 grams = 1 kilogram)
        /// - Litre: 1 (base unit for volume)
        /// - Milliliter: 1000 (1000 milliliters = 1 litre)
        /// </summary>
        public decimal BaseAmount { get; set; }

        /// <summary>
        /// Type of measurement (weight, volume, count, etc.)
        /// </summary>
        public string? MeasurementType { get; set; }

        /// <summary>
        /// Indicates if this is a standard/system-provided unit
        /// </summary>
        public bool IsStandard { get; set; }

        /// <summary>
        /// When the unit was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
