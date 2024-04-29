<template>
    <div class="page_container">
        <el-tabs type="border-card" class="tab_container">
            <div class="personal_info_form tab_pane">
                    <div class="title">Thông tin cá nhân</div>
                    <div class="form-body">
                        <el-form
                            :model="entity"
                            label-width="auto"
                            label-position="top"
                            size="default"
                            :disabled="form.mode === 'view'"
                        >
                            <div class="flex-row cg-4">
                                <div class="form-group fl-1">
                                    <el-form-item label="Mã sinh viên">
                                        <el-input v-model="entity.studentCode" disabled />
                                    </el-form-item>
                                </div>

                                <div class="form-group fl-4">
                                    <el-form-item label="Họ và tên">
                                        <el-input v-model="entity.studentName" disabled />
                                    </el-form-item>
                                </div>
                            </div>
                            <div class="flex-row cg-4">
                                <div class="form-group fl-1">
                                    <el-form-item label="Khoa">
                                        <el-input v-model="entity.facultyName" disabled />
                                    </el-form-item>
                                </div>

                                <div class="form-group fl-1">
                                    <el-form-item label="Chuyên ngành">
                                        <el-input v-model="entity.major" disabled/>
                                    </el-form-item>
                                </div>
                                <div class="form-group fl-1">
                                    <el-form-item label="Lớp">
                                        <el-input v-model="entity.class" disabled />
                                    </el-form-item>
                                </div>
                            </div>

                            <div class="flex-row cg-4">
                                <div class="form-group fl-1">
                                    <el-form-item
                                        label="Điểm trung bình tích lũy"
                                    >
                                        <el-input v-model="entity.gpa" disabled />
                                    </el-form-item>
                                </div>

                                <div class="form-group fl-1"></div>
                                <div class="form-group fl-1"></div>
                            </div>

                            <div class="flex-row cg-4">
                                <div class="form-group fl-1">
                                    <el-form-item label="Số điện thoại">
                                        <el-input v-model="entity.phoneNumber"/>
                                    </el-form-item>
                                </div>

                                <div class="form-group fl-1">
                                    <el-form-item label="Địa chỉ email">
                                        <el-input v-model="entity.email" />
                                    </el-form-item>
                                </div>
                                <div class="form-group fl-1"></div>
                            </div>

                            <div class="form-group fl-1">
                                <el-form-item label="Giới thiệu về bản thân">
                                    <el-input
                                        v-model="entity.description"
                                        type="textarea"
                                        :rows="8"
                                    />
                                </el-form-item>
                            </div>
                        </el-form>
                        <div class="flex-row cg-4 btn-container" v-if="form.mode === 'view'">
                            <el-button type="primary" @click="form.mode = 'edit'">
                                Cập nhật
                            </el-button>
                        </div>
                        <div class="flex-row cg-4 btn-container" v-if="form.mode === 'edit'">
                            <el-button type="primary" @click="btnConfirmOnClick">
                                Đồng ý
                            </el-button>
                            <el-button @click="btnCancelEditOnClick">
                                Hủy
                            </el-button>
                        </div>
                    </div>
                </div>
        </el-tabs>
    </div>
</template>
<script setup>
import { ref } from "vue";
import { useStudentStore, useAuthStore } from "@/stores";
import { httpClient } from "@/helpers";
import { storeToRefs } from "pinia";
import { ElMessage } from "element-plus";
const entityStore = useStudentStore();
const authStore = useAuthStore();
const {loading} = storeToRefs(entityStore);

const entity = ref({});
const oldEntity = ref({});
const form = ref({
    mode: 'view',
});


initData();


async function initData() {
    const userId = authStore.loginInfo?.user.userId;
    const e = await entityStore.getById(userId);
    entity.value = e;
    oldEntity.value = {...e};
}

function btnCancelEditOnClick() {
    form.value.mode = 'view';
    entity.value = {...oldEntity.value};
}

async function btnConfirmOnClick() {
    try {
        const result = await entityStore.update(entity.value);
        if (result) {
            ElMessage.success('Cập nhật thông tin thành công');
            form.value.mode = 'view';
            oldEntity.value = {...entity.value};
        } else {
            ElMessage.error('Có lỗi xảy ra');
        }
    } catch (error) {
        ElMessage.error('Có lỗi xảy ra');
    };
}






</script>
<style scoped>
.page_container {
    width: 100%;
    height: 100%;
    background-color: #fff;
    border-radius: 4px;
}
.personal_info_form {
    padding: 36px;
    margin-left: 20%;
    min-width: 500px;
    width: 60%;
}
.title {
    font-size: 24px;
    font-weight: 600;
}
.form-body {
    margin-top: 48px;
}
.tab_container {
    height: 100%;
}

.btn-container {
    flex-direction: row-reverse;
}
</style>
