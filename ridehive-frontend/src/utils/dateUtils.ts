// Utility functions for date handling in posts

export function formatDateForDisplay(dateString: string): string {
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
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

// Group consecutive time slots into ranges
export function groupTimeSlots(timeSlots: string[]): Array<{ start: string; end: string; display: string }> {
  if (timeSlots.length === 0) return [];
  
  const sorted = sortTimeSlots(timeSlots);
  const groups: Array<{ start: string; end: string; display: string }> = [];
  
  let currentGroup = {
    start: sorted[0]!,
    end: sorted[0]!
  };
  
  for (let i = 1; i < sorted.length; i++) {
    const currentSlot = new Date(sorted[i]!);
    const previousSlot = new Date(sorted[i - 1]!);
    
    // Check if slots are consecutive (within 1 hour)
    const timeDiff = currentSlot.getTime() - previousSlot.getTime();
    const oneHour = 60 * 60 * 1000;
    
    if (timeDiff === oneHour) {
      // Consecutive slot, extend current group
      currentGroup.end = sorted[i]!;
    } else {
      // Non-consecutive slot, finish current group and start new one
      groups.push({
        start: currentGroup.start,
        end: currentGroup.end,
        display: formatTimeRange(currentGroup.start, currentGroup.end)
      });
      
      currentGroup = {
        start: sorted[i]!,
        end: sorted[i]!
      };
    }
  }
  
  // Add the last group
  groups.push({
    start: currentGroup.start,
    end: currentGroup.end,
    display: formatTimeRange(currentGroup.start, currentGroup.end)
  });
  
  return groups;
}

// Format a time range for display
function formatTimeRange(start: string, end: string): string {
  const startDate = new Date(start);
  const endDate = new Date(end);
  
  const formatOptions: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  };
  
  const timeOptions: Intl.DateTimeFormatOptions = {
    hour: '2-digit',
    minute: '2-digit'
  };
  
  // If same date, show: "Oct 20, 2025 - 9:00 AM to 5:00 PM"
  if (startDate.toDateString() === endDate.toDateString()) {
    if (startDate.getTime() === endDate.getTime()) {
      // Single time slot
      return `${startDate.toLocaleDateString('en-US', formatOptions)} at ${startDate.toLocaleTimeString('en-US', timeOptions)}`;
    } else {
      // Same day range
      return `${startDate.toLocaleDateString('en-US', formatOptions)} from ${startDate.toLocaleTimeString('en-US', timeOptions)} to ${endDate.toLocaleTimeString('en-US', timeOptions)}`;
    }
  } else {
    // Different days
    return `${startDate.toLocaleDateString('en-US', formatOptions)} ${startDate.toLocaleTimeString('en-US', timeOptions)} - ${endDate.toLocaleDateString('en-US', formatOptions)} ${endDate.toLocaleTimeString('en-US', timeOptions)}`;
  }
}