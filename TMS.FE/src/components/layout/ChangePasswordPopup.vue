<template>
    <el-dialog
        v-model="visible"
        title="Đổi mật khẩu"
        width="500"
        :close-on-click-modal="false"
        @open="dialogOnOpen"
    >
    <el-form
        ref="refForm"
        label-position="left"
        label-width="auto"
        :model="entity"
        style="max-width: 600px"
        :rules="rules"
    >
        <el-form-item label="Mật khẩu cũ" prop="oldPass">
            <el-input v-model="entity.oldPass" type="password"/>
        </el-form-item>
        <el-form-item label="Mật khẩu mới" prop="newPass">
            <el-input v-model="entity.newPass" type="password" />
        </el-form-item>
        <el-form-item label="Nhập lại mật khẩu mới" prop="confirmPass">
            <el-input v-model="entity.confirmPass" type="password" />
        </el-form-item>
    </el-form>
    
    <template #footer>
      <div class="dialog-footer">
        <el-button >Hủy</el-button>
        <el-button type="primary"
            @click="btnConfirmOnClick"
        >
          Đồng ý
        </el-button>
      </div>
    </template>
    

    </el-dialog>
</template>
<script setup>
import {ref, reactive} from "vue";
import { httpClient } from "@/helpers";
import $api from "@/api";
import { ElMessage } from "element-plus";

const API = $api.user;



const visible = defineModel("visible");
const refForm = ref(null);
const entity = ref({
    oldPass: "",
    newPass: "",
    confirmPass: ""
});

const rules = {
    oldPass: [
        {required: true, message: "Không được để trống", trigger: "change"}
    ],
    newPass: [
        {required: true, message: "Không được để trống", trigger: "change"}
    ],
    confirmPass: [        
        { validator: validateConfirmPassword, trigger: 'blur' },
        { required: true, message: "Không được để trống", trigger: ["change", "blur"]},
    ]
};

async function resetForm() {
    refForm.value.resetFields();
    entity.value = {
        oldPass: "",
        newPass: "",
        confirmPass: ""
    }
}

async function dialogOnOpen() {
    await resetForm();
}

async function btnConfirmOnClick() {
    refForm.value.validate(async (valid) => {
        if (valid) {
            try {
                const response = await httpClient.post(API.changePassword, entity.value);
                if (response.success && response.data) {
                    ElMessage.success("Đổi mật khẩu thành công");
                    await resetForm();
                } else if (response.success && response.errorCode){
                    ElMessage.error(response.message);
                }
            } catch(error) {
                console.log(error);
                ElMessage.error('Có lỗi xảy ra. Vui lòng thử lại.');
            }
        } else {}
    });
}

function validateConfirmPassword(rule, value, callback) {
    if (value.length > 0 && value !== entity.value.newPass) {
        callback(Error("Mật khẩu không khớp"));
    } else {
        callback();
    }
}
</script>
<style scoped>
:deep(.el-form-item__label:before) {
    content: unset !important;
}
</style>