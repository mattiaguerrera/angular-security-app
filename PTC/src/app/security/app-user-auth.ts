import { UserAuthBase } from "../shared/security/user-auth-base";
import { AppUserClaim } from "./app-user-claim";

export class AppUserAuth extends UserAuthBase {
  claims: AppUserClaim[] = [];

  override init(): void {
    super.init();    
  }

  getValueOfProperty(obj: any, key:string):boolean {
    let ret = obj[key];
    return ret;
  }  
}
