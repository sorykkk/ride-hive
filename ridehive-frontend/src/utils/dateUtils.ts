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