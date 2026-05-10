/**
 * Utility for sorting arrays by a specific property
 */
export type SortDirection = 'asc' | 'desc' | null;

export interface SortState {
  column: string | null;
  direction: SortDirection;
}

export class SortUtil {
  /**
   * Sort an array by a specific column/property
   * @param data Array to sort
   * @param column Column/property name to sort by
   * @param direction Sort direction ('asc', 'desc', or toggle if null)
   * @param currentSortColumn Current sort column for toggle logic
   * @param currentDirection Current sort direction for toggle logic
   * @returns Sorted array
   */
  static sort<T>(
    data: T[],
    column: keyof T,
    direction?: SortDirection,
    currentSortColumn?: keyof T | null,
    currentDirection?: SortDirection
  ): T[] {
    // Determine sort direction
    let sortDirection: SortDirection;
    if (direction !== undefined && direction !== null) {
      sortDirection = direction;
    } else if (currentSortColumn === column && currentDirection === 'asc') {
      // Toggle from asc to desc
      sortDirection = 'desc';
    } else {
      // Default to asc
      sortDirection = 'asc';
    }

    // If no direction, clear sort
    if (!sortDirection) {
      return data;
    }

    const sorted = [...data].sort((a, b) => {
      const aVal = a[column];
      const bVal = b[column];

      // Handle null/undefined values
      if (aVal == null && bVal == null) return 0;
      if (aVal == null) return sortDirection === 'asc' ? 1 : -1;
      if (bVal == null) return sortDirection === 'asc' ? -1 : 1;

      // Compare values
      let comparison = 0;
      if (typeof aVal === 'string' && typeof bVal === 'string') {
        comparison = aVal.localeCompare(bVal, 'ar');
      } else if (typeof aVal === 'number' && typeof bVal === 'number') {
        comparison = aVal - bVal;
      } else if (aVal instanceof Date && bVal instanceof Date) {
        comparison = aVal.getTime() - bVal.getTime();
      } else {
        comparison = String(aVal).localeCompare(String(bVal), 'ar');
      }

      return sortDirection === 'asc' ? comparison : -comparison;
    });

    return sorted;
  }

  /**
   * Get the sort state after toggling a column
   */
  static toggleSort(column: string, currentColumn: string | null, currentDirection: SortDirection): SortState {
    if (currentColumn === column) {
      if (currentDirection === 'asc') {
        return { column, direction: 'desc' };
      } else {
        return { column: null, direction: null };
      }
    }
    return { column, direction: 'asc' };
  }
}
