describe("Testes com Auth", () => {
  it("Login com senha errada", () => {
    cy.visit("https://autenticador-dev.carreira.com.br");

    cy.intercept("POST", "**/api/account/login/").as("getLogin");

    cy.get("#label-email")
      .type("thiagosilverio@carreira.lan")
      .should("have.value", "thiagosilverio@carreira.lan");

    cy.get("#label-password").type("senhaErrada");

    cy.get(".button-login").click();

    cy.wait("@getLogin").then((intercept) => {
      cy.log(intercept);
      expect(intercept.response.statusCode).to.equal(400);
    });
  });

  it("Login com senha correta", () => {
    cy.visit("https://autenticador-dev.carreira.com.br");

    cy.intercept("POST", "**/api/account/login/").as("getLogin");

    cy.get("#label-email")
      .type("thiagosilverio@carreira.lan")
      .should("have.value", "thiagosilverio@carreira.lan");

    cy.get("#label-password").type("Carreira@2020");

    cy.get(".button-login").click();

    cy.wait("@getLogin").then((intercept) => {
      cy.log(intercept);
      expect(intercept.response.statusCode).to.equal(200);
    });
  });

  it("Login com senha correta e selecionando Salary Mark", () => {
    cy.visit("https://autenticador-dev.carreira.com.br");

    cy.get("#label-email")
      .type("thiagosilverio@carreira.lan")
      .should("have.value", "thiagosilverio@carreira.lan");

    cy.get("#label-password").type("Carreira@2020");

    cy.get(".button-login").click();

    cy.wait(2000);

    cy.get(".card-h").then((res) => {
      cy.log(res);
      res[0].click();
    });

    cy.url().should("include", "salarymark-dev");
  });
});
