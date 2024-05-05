<template>
    <el-dialog 
        v-model="dialogVisible" 
        :title="form.title" 
        width="800" draggable 
        :close-on-click-modal="false"
        @close="dialogOnClose"
        top="8vh"
        >  
        <div class="dialog-body"
            v-loading.fullscreen="loading ? {
                lock: true,
                text: '',
                background: 'rgba(0, 0, 0, 0.1)',
            } : false"
        >
            <el-form
                :model="entity"
                label-width="auto"
                label-position="top"
                size="default"
                :disabled="form.mode === 'view'"
            >
            <el-tabs v-model="curTabname">
                <el-tab-pane label="Thông tin chung" name="tenantInfo">
                    <div class="flex-row cg-4">
                        <div class="form-group">
                            <el-form-item label="Mã khách hàng">
                                <el-input v-model="entity.tenantCode" />
                            </el-form-item>
                        </div>

                        <div class="form-group fl-1">
                            <el-form-item label="Tên khách hàng">
                                <el-input v-model="entity.tenantName" />
                            </el-form-item>
                        </div>
                    </div>
                    <div class="flex-row cg-4">
                        <div class="form-group fl-3">
                            <el-form-item label="Địa chỉ">
                                <el-input v-model="entity.address" />
                            </el-form-item>
                        </div>

                        <div class="form-group fl-1">
                            <el-form-item label="Số điện thoại">
                                <el-input v-model="entity.phoneNumber" />
                            </el-form-item>
                        </div>
                        <div class="form-group fl-2">
                            <el-form-item label="Email">
                                <el-input v-model="entity.email" />
                            </el-form-item>
                        </div>
                    </div>
                    <div class="flex-row cg-4">
                        <div class="form-group fl-2">
                            <el-form-item label="Tên người đại diện">
                                <el-input v-model="entity.surrogateName" />
                            </el-form-item>
                        </div>

                        <div class="form-group fl-1">
                            <el-form-item label="Số điện thoại NĐD">
                                <el-input v-model="entity.surrogatePhoneNumber" />
                            </el-form-item>
                        </div>
                        <div class="form-group fl-2">
                            <el-form-item label="Email NĐD">
                                <el-input v-model="entity.surrogateEmail" />
                            </el-form-item>
                        </div>
                    </div>
                    <div class="flex-row cg-4">
                        <div class="form-group fl-2">
                            <el-form-item label="Trạng thái">
                                <el-input v-model="TenantStatus[entity.status]" :disabled="true" />
                            </el-form-item>
                        </div>
                    </div>

                </el-tab-pane>
                <el-tab-pane label="Thiết lập kết nối" name="tenantSetting">
                    <div class="flex-row">
                        <div class="form-group fl-1">
                            <el-form-item label="Tên Domain">
                                <el-input v-model="entity.domain" :disabled="entity.status === 2"/>
                            </el-form-item>
                        </div>
                    </div>
                    <div class="flex-row cg-4">
                        <div class="form-group">
                            <el-form-item label="Sử dụng CSDL mặc định">
                                <el-switch v-model="entity.autoCreateDB" :disabled="entity.status === 2"/>
                            </el-form-item>
                        </div>
                        <div class="form-group fl-1">
                            <el-form-item label="Connection string">
                                <el-input v-model="entity.dbConnection" :disabled="entity.autoCreateDB"/>
                            </el-form-item>
                        </div>
                        <div class="form-group">
                            <el-form-item label="Tên Database">
                                <el-input v-model="entity.dbName" :disabled="entity.autoCreateDB"/>
                            </el-form-item>
                        </div>
                    </div>
                    <div class="flex-row cg-4">
                        <div class="form-group">
                            <el-form-item label="Sử dụng MinIO mặc định">
                                <el-switch v-model="entity.autoCreateMinio" :disabled="entity.status === 2" />
                            </el-form-item>
                        </div>
                        <div class="form-group fl-1">
                            <el-form-item label="Endpoint">
                                <el-input v-model="entity.minioEndpoint" :disabled="entity.autoCreateMinio" />
                            </el-form-item>
                        </div>
                        
                    </div>  
                    <div class="flex-row cg-4">
                        <div class="form-group fl-1">
                            <el-form-item label="Access key">
                                <el-input v-model="entity.minioAccessKey" :disabled="entity.autoCreateMinio"/>
                            </el-form-item>
                        </div>
                        <div class="form-group fl-1">
                            <el-form-item label="Secret key">
                                <el-input v-model="entity.minioSecretKey" :disabled="entity.autoCreateMinio"/>
                            </el-form-item>
                        </div>
                        <div class="form-group fl-1">
                            <el-form-item label="Bucket name">
                                <el-input v-model="entity.minioBucketName" :disabled="entity.autoCreateMinio"/>
                            </el-form-item>
                        </div>
                    </div>  
                </el-tab-pane>
            </el-tabs>
              
            </el-form>

        </div>

        <template #footer>
            <div class="dialog-footer">
                <div class="form-add-edit-footer" v-if="form.mode !== 'view'">
                    <el-button v-show="curTabname === 'tenantSetting'" type="" @click="checkTenantConnection">Kiểm tra kết nối</el-button>
                    <el-button @click="btnCancelOnClick">Hủy</el-button>
                    <el-button type="primary" @click="btnConfirmOnClick">
                        Đồng ý
                    </el-button>
                </div>
                <div v-else class="form-view-footer">
                    <el-button v-show="curTabname === 'tenantSetting'" type="" @click="checkTenantConnection">Kiểm tra kết nối</el-button>
                    <el-button v-show="curTabname === 'tenantSetting' && entity.status === 0" type="primary" @click="btnActiveTenantOnClick">Kích hoạt</el-button>
                    <el-button v-show="curTabname !== 'tenantSetting'" type="primary" @click="btnEditOnClick">
                        Sửa
                    </el-button>
                    <el-button @click="btnCloseOnClick">Đóng</el-button>
                </div>
            </div>
        </template>
    </el-dialog>
</template>

<script setup>
    import { ref } from 'vue';
    import { useRouter, useRoute } from 'vue-router';
    import {useTenantStore} from '@/stores';
    import {useForm} from '@/composables'
    import { storeToRefs } from 'pinia';
    import {TenantStatus} from '@/common/enum';
    import { ElMessage, ElMessageBox } from 'element-plus'

    const router = useRouter();
    const route = useRoute();
    const tenantStore = useTenantStore();

    const {loading} = storeToRefs(tenantStore);

    const dialogVisible = ref(true);

    const {form, entity} = useForm('Khách hàng');

    const curTabname = ref('tenantInfo');


    initData();

    async function initData() {
        if (form.value.mode === 'edit' || form.value.mode === 'view') {
            const tenantId = route.params.id;
            const tenant = await tenantStore.getById(tenantId);
            entity.value = {...tenant};
        } else if (form.value.mode === 'add') {
            const newEntity = await tenantStore.getNew();
            entity.value = {...newEntity};
        }
    }

    function gotoPageList() {
        router.push('/tenant');
    }

    async function btnConfirmOnClick() {
        if (form.value.mode === 'add') {
            await tenantStore.insert({...entity.value});
        } else {
            await tenantStore.update({...entity.value});
        }
    }

    async function btnEditOnClick() {
        router.push(`/tenant/edit/${entity.value.tenantId}`);
        form.value.mode = 'edit';
        form.value.title = 'Sửa Khách hàng';
    }

    function btnCloseOnClick() {
        gotoPageList();
    }

    function dialogOnClose() {
        gotoPageList();
    }

    function btnCancelOnClick() {
        gotoPageList();
    }

    async function checkTenantConnection() {
        const result = await tenantStore.checkConnection({...entity.value});
        if (!result.ErrorCode) {  
            ElMessage.success('Thử kết nối thành công');
        } else {
            ElMessage.error('Thử kết nối thất bại: ' + result.Message);
        }
    }

    async function btnActiveTenantOnClick() {
        await tenantStore.activeTenant({...entity.value});
    }



</script>

<style scoped>
    .subtitle {
        font-weight: bold;
        font-size: 16px;
        margin-bottom: 8px;
        margin-top: 8px;
    }

</style>