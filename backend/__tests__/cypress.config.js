const { defineConfig } = require("cypress");
const { launchUrl } = require('../LoyaltyApi/Properties/launchSettings.json');

module.exports = defineConfig({
  e2e: {
    setupNodeEvents(on, config) {
      // implement node event listeners here
    },
    specPattern: [
      "e2e/**/*.{cy,spec}.{js,ts,jsx,tsx}",
    ],
    baseUrl: launchUrl
  },
});
