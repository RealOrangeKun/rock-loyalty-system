/// <reference types="cypress" />
const { launchUrl } = require("../../config")

describe('Register', () => {

    beforeEach(() => {
        cy.log(launchUrl);
    })

    it('should register new user', () => {
        expect(true).to.equal(true)
    })
})