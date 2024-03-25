<template>
    <el-dialog 
        v-model="dialogVisible" 
        :title="form.title" 
        width="800" draggable 
        :close-on-click-modal="false"
        @close="dialogOnClose"
        >  
        <div class="dialog-body"
            v-loading.fullscreen="loading ? {
                lock: true,
                text: '',
                background: 'rgba(0, 0, 0, 0.1)',
            } : false"
        >
            <el-form
                ref="refForm"
                :model="entity"
                label-width="auto"
                label-position="top"
                size="default"
            >
                <div class="subtitle">Thông tin chung</div>
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
                <div class="subtitle">Thiết lập</div>
                <div class="flex-row cg-4">
                    <div class="form-group">
                        <el-form-item label="Tự động tạo cơ sở dữ liệu">
                            <el-switch v-model="entity.autoCreateDB" />
                        </el-form-item>
                    </div>
                    <div class="form-group fl-1" v-if="!entity.autoCreateDB">
                        <el-form-item label="Connection string">
                            <el-input v-model="entity.dBConnection" />
                        </el-form-item>
                    </div>
                </div>
                <div class="flex-row">
                    <div class="form-group fl-1">
                        <el-form-item label="Tên Domain">
                            <el-input v-model="entity.domain" />
                        </el-form-item>
                    </div>
                </div>
                
            </el-form>

        </div>

        <template #footer>
            <div class="dialog-footer">
                <el-button @click="btnCancelOnClick">Hủy</el-button>
                <el-button type="primary" @click="btnConfirmOnClick">
                    Đồng ý
                </el-button>
            </div>
        </template>
    </el-dialog>
</template>

<script setup>
    import { ref } from 'vue';
    import { useRouter, useRoute } from 'vue-router';
    import {useTenantStore} from '@/stores';
import { storeToRefs } from 'pinia';

    const router = useRouter();
    const route = useRoute();
    const tenantStore = useTenantStore();

    const {loading} = storeToRefs(tenantStore);

    const dialogVisible = ref(true);

    const form = ref({
        title: '',
        type: '',
    });
    const refForm = ref(null);

    const entity = ref({
        tenantId: null,
        tenantName: '',
        tenantCode: '',
        dBConnection: '',
        surrogateName: '',
        surrogatePhoneNumber: '',
        surrogateEmail: '',
        autoCreateDB: true,
        phoneNumber: '',
        address: '',
        email: '',
        domain: '',
        state: 1,
    });



    initForm();

    function initForm() {
        if (route.path.includes('new')) {
            form.value.title = 'Thêm mới khách hàng';
            form.value.type = 'add';
        }
        console.log(form.value.title)
    }





    function gotoTenantList() {
        router.push('/tenant');
    }

    async function btnConfirmOnClick() {
        if (form.value.type === 'add') {
            await tenantStore.insert({...entity.value});
        } else {
            await tenantStore.update({...entity.value});
        }
    }

    function dialogOnClose() {
        gotoTenantList();
    }

    function btnCancelOnClick() {
        gotoTenantList();
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