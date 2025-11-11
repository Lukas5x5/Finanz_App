/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/FinanceApp.Web/**/*.{razor,html,cshtml}",
    "./src/FinanceApp.Mobile/**/*.{razor,html,cshtml}"
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        primary: {
          light: '#007AFF',
          dark: '#0A84FF',
        },
        secondary: {
          light: '#5856D6',
          dark: '#5E5CE6',
        },
        success: {
          light: '#34C759',
          dark: '#32D74B',
        },
        warning: {
          light: '#FF9500',
          dark: '#FF9F0A',
        },
        danger: {
          light: '#FF3B30',
          dark: '#FF453A',
        },
        background: {
          light: '#FFFFFF',
          dark: '#000000',
        },
        surface: {
          light: '#F2F2F7',
          dark: '#1C1C1E',
        },
        border: {
          light: '#C6C6C8',
          dark: '#38383A',
        },
      },
      fontFamily: {
        sans: ['-apple-system', 'BlinkMacSystemFont', 'Segoe UI', 'Roboto', 'sans-serif'],
      },
      borderRadius: {
        'apple': '0.75rem',
      },
      boxShadow: {
        'apple': '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
        'apple-lg': '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)',
      },
    },
  },
  plugins: [],
}
