package com.senabo.domain.emergency.entity;

import static com.querydsl.core.types.PathMetadataFactory.*;

import com.querydsl.core.types.dsl.*;

import com.querydsl.core.types.PathMetadata;
import javax.annotation.processing.Generated;
import com.querydsl.core.types.Path;
import com.querydsl.core.types.dsl.PathInits;


/**
 * QEmergency is a Querydsl query type for Emergency
 */
@Generated("com.querydsl.codegen.DefaultEntitySerializer")
public class QEmergency extends EntityPathBase<Emergency> {

    private static final long serialVersionUID = 2094221502L;

    private static final PathInits INITS = PathInits.DIRECT2;

    public static final QEmergency emergency = new QEmergency("emergency");

    public final com.senabo.common.audit.QBaseEntity _super = new com.senabo.common.audit.QBaseEntity(this);

    //inherited
    public final DateTimePath<java.time.LocalDateTime> createTime = _super.createTime;

    public final NumberPath<Long> id = createNumber("id", Long.class);

    public final com.senabo.domain.member.entity.QMember memberId;

    public final BooleanPath solved = createBoolean("solved");

    public final EnumPath<EmergencyType> type = createEnum("type", EmergencyType.class);

    //inherited
    public final DateTimePath<java.time.LocalDateTime> updateTime = _super.updateTime;

    public QEmergency(String variable) {
        this(Emergency.class, forVariable(variable), INITS);
    }

    public QEmergency(Path<? extends Emergency> path) {
        this(path.getType(), path.getMetadata(), PathInits.getFor(path.getMetadata(), INITS));
    }

    public QEmergency(PathMetadata metadata) {
        this(metadata, PathInits.getFor(metadata, INITS));
    }

    public QEmergency(PathMetadata metadata, PathInits inits) {
        this(Emergency.class, metadata, inits);
    }

    public QEmergency(Class<? extends Emergency> type, PathMetadata metadata, PathInits inits) {
        super(type, metadata, inits);
        this.memberId = inits.isInitialized("memberId") ? new com.senabo.domain.member.entity.QMember(forProperty("memberId")) : null;
    }

}

