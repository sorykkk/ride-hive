// Utility functions for date handling in posts

export function formatDateForDisplay(dateString: string): string {
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  });
}

export function formatDateForAPI(date: Date): string {
  return date.toISOString();
}

export function parseAPIDate(dateString: string): Date {
  return new Date(dateString);
}

export function isDateInFuture(dateString: string): boolean {
  return new Date(dateString) > new Date();
}

export function sortTimeSlots(timeSlots: string[]): string[] {
  return timeSlots
    .map(slot => ({ original: slot, date: new Date(slot) }))
    .sort((a, b) => a.date.getTime() - b.date.getTime())
    .map(item => item.original);
}

export function getAvailableDates(timeSlots: string[]): string[] {
  const now = new Date();
  return timeSlots.filter(slot => new Date(slot) > now);
}

// Generate all dates between start and end (inclusive)
export function generateDatesBetween(startDate: Date, endDate: Date): Date[] {
  const dates: Date[] = [];
  const currentDate = new Date(startDate);
  
  // Ensure start is not after end
  if (currentDate > endDate) {
    return [];
  }
  
  while (currentDate <= endDate) {
    dates.push(new Date(currentDate));
    currentDate.setDate(currentDate.getDate() + 1);
  }
  
  return dates;
}

// Generate ISO string dates between start and end (inclusive)
export function generateDateRangeStrings(startDate: Date, endDate: Date): string[] {
  return generateDatesBetween(startDate, endDate).map(date => formatDateForAPI(date));
}

// Group consecutive date slots into ranges (date-only, no time)
export function groupTimeSlots(timeSlots: string[]): Array<{ start: string; end: string; display: string }> {
  if (timeSlots.length === 0) return [];
  
  const sorted = sortTimeSlots(timeSlots);
  
  // Return each date individually to avoid overlaps
  return sorted.map(slot => ({
    start: slot,
    end: slot,
    display: formatDateOnly(slot)
  }));
}

// Format a single date for display (date-only, no time)
function formatDateOnly(dateString: string): string {
  const date = new Date(dateString);
  
  const formatOptions: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  };
  
  return date.toLocaleDateString('en-US', formatOptions);
}