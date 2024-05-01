<template>
    <div class="page_container">
        <el-tabs type="border-card" class="tab_container">
            <div class="personal_info_form tab_pane">
                    <div class="title">Thiết lập</div>
                    <div class="action flex-row">
                        <div class="subtitle">Thiết lập thời gian đăng ký khóa luận tốt nghiệp</div>
                        <div v-if="form.mode === 'view'">
                            <el-button type="primary" @click="form.mode = 'edit'">
                                Cập nhật
                            </el-button>
                        </div>
                        <div v-if="form.mode === 'edit'">
                            <el-button type="primary" @click="btnConfirmOnClick">
                                Đồng ý
                            </el-button>
                            <el-button @click="btnCancelEditOnClick">
                                Hủy
                            </el-button>
                        </div>
                    </div>
                    {{ value1 }}
                    <div class="form-body">
                        <el-form
                            :model="entity"
                            label-width="auto"
                            label-position="top"
                            size="default"
                            :disabled="form.mode === 'view'"
                        >
                            <div class="flex-row cg-4">
                                <div class="form-group fl-2">
                                    <el-form-item label="Thời gian mở đăng ký khóa luận">
                                        <el-date-picker
                                            v-model="value1"
                                            type="datetimerange"
                                            range-separator="Đến"
                                            start-placeholder="Ngày bắt đầu"
                                            end-placeholder="Ngày kết thúc"
                                            format="DD/MM/YYYY HH:mm:ss"
                                            />
                                    </el-form-item>       
                                </div>
                                <div class="form-group fl-1"></div>
                            </div>
                            <div class="flex-row cg-4 mt-5">
                                <div class="form-group fl-2">
                                    <el-form-item label="Thời gian chỉnh sửa tên đề tài">
                                        <el-date-picker
                                            v-model="value2"
                                            type="datetimerange"
                                            range-separator="Đến"
                                            start-placeholder="Ngày bắt đầu"
                                            end-placeholder="Ngày kết thúc"
                                            format="DD/MM/YYYY HH:mm:ss"
                                            />
                                    </el-form-item>       
                                </div>
                                <div class="form-group fl-1"></div>
                            </div>
                        </el-form>
                        
                    </div>
                </div>
        </el-tabs>
    </div>
</template>
<script setup>
import { ref } from "vue";
import { useTeacherStore, useAuthStore } from "@/stores";
import { httpClient } from "@/helpers";
import { storeToRefs } from "pinia";
import { ElMessage } from "element-plus";
const entityStore = useTeacherStore();
const authStore = useAuthStore();
const {loading} = storeToRefs(entityStore);

const entity = ref({});
const oldEntity = ref({});
const form = ref({
    mode: 'view',
});

const value1 = ref(null);
const value2 = ref(null);


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

.action {
    margin-top: 40px;
    justify-content: space-between;
}

.subtitle {
    line-height: 36px;
}
</style>
