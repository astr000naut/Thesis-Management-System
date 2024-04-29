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
                            :disabled="true"
                        >
                            <div class="flex-row cg-4">
                                <div class="form-group fl-1">
                                    <el-form-item label="Mã sinh viên">
                                        <el-input v-model="entity" disabled />
                                    </el-form-item>
                                </div>

                                <div class="form-group fl-4">
                                    <el-form-item label="Họ và tên">
                                        <el-input v-model="entity" disabled />
                                    </el-form-item>
                                </div>
                            </div>
                            <div class="flex-row cg-4">
                                <div class="form-group fl-1">
                                    <el-form-item label="Khoa">
                                        <el-input v-model="entity" disabled />
                                    </el-form-item>
                                </div>

                                <div class="form-group fl-1">
                                    <el-form-item label="Chuyên ngành">
                                        <el-input v-model="entity" />
                                    </el-form-item>
                                </div>
                                <div class="form-group fl-1">
                                    <el-form-item label="Lớp">
                                        <el-input v-model="entity" />
                                    </el-form-item>
                                </div>
                            </div>

                            <div class="flex-row cg-4">
                                <div class="form-group fl-1">
                                    <el-form-item
                                        label="Điểm trung bình tích lũy"
                                    >
                                        <el-input v-model="entity" disabled />
                                    </el-form-item>
                                </div>

                                <div class="form-group fl-1"></div>
                                <div class="form-group fl-1"></div>
                            </div>

                            <div class="flex-row cg-4">
                                <div class="form-group fl-1">
                                    <el-form-item label="Số điện thoại">
                                        <el-input v-model="entity" disabled />
                                    </el-form-item>
                                </div>

                                <div class="form-group fl-1">
                                    <el-form-item label="Địa chỉ email">
                                        <el-input v-model="entity" disabled />
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
                            <div class="flex-row cg-4 btn-container">
                                <el-button type="primary" @click="() => {}">
                                    Sửa
                                </el-button>
                                <el-button @click="() => {}"> Hủy </el-button>
                            </div>
                        </el-form>
                    </div>
                </div>
        </el-tabs>
    </div>
</template>
<script setup>
import { ref } from "vue";
const entity = ref("");
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
    margin-left: 280px;
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
