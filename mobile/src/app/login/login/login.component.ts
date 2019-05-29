import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatSnackBar } from '@angular/material';

import { AccountService, LoginModel } from '../../services/account.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

    public model: LoginModel = { };

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private snackBar: MatSnackBar,
        private accountSvc: AccountService
    ) { }

    public ngOnInit(): void {
    }

    public async login(): Promise<void> {
        try {
            await this.accountSvc.login(this.model);
            await this.accountSvc.getInfo();
            const returnUrl = this.route.snapshot.params['returnUrl'] || 'home';
            this.router.navigate(
                ['/' + returnUrl],
                { replaceUrl: true }
            );
        }
        catch (ex) {
            console.error(ex);
            this.snackBar.open(ex.error, '确定', { duration: 3000 });
        }
    }

}