const { defineConfig } = require("cypress");

module.exports = defineConfig({
  e2e: {
    baseUrl: "http://localhost:5152",
    specPattern: "cypress/e2e/**/*.spec.js", 
    viewportWidth: 1280,
    viewportHeight: 720,
    setupNodeEvents(on, config) {
      // implement node event listeners here
    },
  },
});
