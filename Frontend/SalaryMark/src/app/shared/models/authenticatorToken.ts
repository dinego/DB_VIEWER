import { ProductTypeEnum } from './token';

export interface IAuthenticatorToken{
  token: string;
  isAuthenticated: boolean;
  expiresAt: Date;
}

export class AuthenticatorToken implements IAuthenticatorToken
{
  token: string;
  isAuthenticated: boolean;
  expiresAt: Date;
  constructor(snapshotAuthenticator?: IAuthenticatorToken) {
    Object.assign(this, snapshotAuthenticator);
  }
}

export interface IAccessToken{
  accessTokenAuth: IAuthenticatorToken,
  accessTokenCS: IAuthenticatorToken,
  accessTokenSM: IAuthenticatorToken,
  products: ProductTypeEnum[]
}
