describe("Sandbox - Testando comandos", () => {
  it("Testando environment", () => {
    cy.log(Cypress.env("baseURL").dev);
    cy.visit(Cypress.env("baseURL").staging);
  });
});
