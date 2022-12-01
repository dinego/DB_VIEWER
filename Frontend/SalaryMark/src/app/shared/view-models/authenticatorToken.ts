export interface IAuthenticatorToken {
  token: string;
  isAuthenticated: boolean;
  expiresAt: Date;
}

export class AuthenticatorToken implements IAuthenticatorToken {
  token: string;
  isAuthenticated: boolean;
  expiresAt: Date;
  constructor(snapshotAuthenticator?: IAuthenticatorToken) {
    Object.assign(this, snapshotAuthenticator);
  }
}
