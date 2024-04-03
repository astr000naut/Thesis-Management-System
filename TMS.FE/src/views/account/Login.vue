<template>
    <div class="login__container flex al-center jc-center">
        <div class="login__form flex-col rg-5">   
            <div class="form__header flex-row cg-5 al-center">
                <div class="header__logo">
                    <Icon icon="simple-icons:microsoftacademic" style="width: 56px; height: 56px; color: #409EFF;"/>
                </div>
                <div class="header__title flex-col rg-2">
                    <div class="text-bold">Hệ thống quản lý khóa luận tốt nghiệp</div>
                    <div class="text-center text-bold">{{ authStore.tenantBaseInfo.tenantName }}</div>
                </div>
            </div>
            <div class="form__body fl-1">
                <Form class="flex-col rg-5" @submit="onSubmit" :validation-schema="schema" v-slot="{ errors, isSubmitting }">
                    <div class="form-group">
                        <label>Tên đăng nhập</label>
                        <Field name="username" type="text" class="form-control" :class="{ 'is-invalid': errors.username }" @update:model-value="loginMessage=''"/>
                        <div class="invalid-feedback">{{ errors.username }}</div>
                    </div>
                    <div class="form-group">
                        <label>Mật khẩu</label>
                        <Field name="password" type="password" class="form-control" :class="{ 'is-invalid': errors.password }" @update:model-value="loginMessage=''" />
                        <div class="invalid-feedback">{{ errors.password }}</div>
                        <div class="login-message invalid-feedback" v-show="loginMessage.length">{{ loginMessage }}</div>
                    </div>
                    <div class="form-group" style="margin-top: 10px;">
                        <button class="formBtn" :disabled="isSubmitting">
                            <div v-show="!isSubmitting" >Đăng nhập</div>
                            <Icon icon="line-md:loading-loop" 
                                v-if="isSubmitting" 
                                style="width: 24px; height: 24px;"
                            >
                            </Icon>
                        </button>
                    </div>
                </Form> 
            </div>
            
        </div>
    </div>


</template>

<script setup>
import { Form, Field } from 'vee-validate';
import * as Yup from 'yup';
import { ref } from 'vue';

import { useAuthStore } from '@/stores';
import { router } from '@/router';

// redirect home if already logged in
const authStore = useAuthStore();
if (authStore.loginInfo) {
    router.push('/');
}


const schema = Yup.object().shape({
    username: Yup.string().required('Tên đăng nhập không được để trống'),
    password: Yup.string().required('Mật khẩu không được để trống')
});

const loginMessage = ref('');


async function onSubmit(values) {
    loginMessage.value = '';
    const { username, password } = values;
    const errorMessage = await authStore.login(username, password);
    if (errorMessage) {
        loginMessage.value = errorMessage;
    }
}

</script>



<style scoped>

    .form-group {
        display: flex;
        flex-direction: column;
        row-gap: 4px;
    }

    .formBtn {
        background-color: #3090df;
        color: white;
        outline: unset;
        border: unset;
        cursor: pointer;
        border-radius: 4px;
    }

    .formBtn:not(:disabled):active {
        background-color: #1e6bb8;
    }

    .invalid-feedback {
        color: red;
    }

    
    .header__title {
        font-size: 20px;
    }

    .login__container {
        border: 1px solid green;
        width: 100%;
        height: 100%;
    }
    .login__form {
        width: 500px;
        background-color: #fff;
        border-radius: 8px;
        padding: 50px;
    }

</style>
